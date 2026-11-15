import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const artSrc = path.resolve(__dirname, "..", "Assets", "_Project", "Art");
const gameAssetsDst = path.resolve(__dirname, "public", "game-assets");
const audioSrc = path.resolve(__dirname, "..", "Assets", "_Project", "Audio", "SFX");
const audioDst = path.resolve(__dirname, "public", "audio");

function copyRecursive(src, dest) {
  if (!fs.existsSync(src)) return;
  fs.mkdirSync(dest, { recursive: true });
  for (const entry of fs.readdirSync(src, { withFileTypes: true })) {
    if (entry.name.endsWith(".meta")) continue;
    const s = path.join(src, entry.name);
    const d = path.join(dest, entry.name);
    if (entry.isDirectory()) {
      copyRecursive(s, d);
    } else if (!fs.existsSync(d)) {
      fs.copyFileSync(s, d);
    }
  }
}

console.log("[sync-assets] Sincronizando recursos de Unity hacia public/game-assets...");
copyRecursive(artSrc, gameAssetsDst);
copyRecursive(audioSrc, audioDst);

// Generar pista musical mars_crafts_trailer_ost.wav si no existe
const ostPath = path.join(audioDst, "mars_crafts_trailer_ost.wav");
if (!fs.existsSync(ostPath)) {
  fs.mkdirSync(audioDst, { recursive: true });
  const sr = 22050;
  const dur = 45;
  const numSamples = sr * dur;
  const buf = Buffer.alloc(44 + numSamples * 2);
  buf.write("RIFF", 0);
  buf.writeUInt32LE(36 + numSamples * 2, 4);
  buf.write("WAVE", 8);
  buf.write("fmt ", 12);
  buf.writeUInt32LE(16, 16);
  buf.writeUInt16LE(1, 20);
  buf.writeUInt16LE(1, 22);
  buf.writeUInt32LE(sr, 24);
  buf.writeUInt32LE(sr * 2, 28);
  buf.writeUInt16LE(2, 32);
  buf.writeUInt16LE(16, 34);
  buf.write("data", 36);
  buf.writeUInt32LE(numSamples * 2, 40);

  const notes = [220, 261.63, 329.63, 392.0, 440.0, 523.25, 659.25, 587.33];
  for (let i = 0; i < numSamples; i++) {
    const t = i / sr;
    const beat = Math.floor(t * 4);
    const freq = notes[beat % notes.length];
    const bass = Math.sin(2 * Math.PI * (freq / 2) * t) * 0.25;
    const lead = Math.sin(2 * Math.PI * freq * t) * 0.2 * Math.exp(-((t * 4) % 1) * 3.5);
    const pad = Math.sin(2 * Math.PI * (freq * 1.5) * t) * 0.12;
    const val = Math.max(-1, Math.min(1, bass + lead + pad));
    buf.writeInt16LE(Math.floor(val * 24000), 44 + i * 2);
  }
  fs.writeFileSync(ostPath, buf);
  console.log("[sync-assets] Banda sonora mars_crafts_trailer_ost.wav generada.");
}
console.log("[sync-assets] Listo.");
