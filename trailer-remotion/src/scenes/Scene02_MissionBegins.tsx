import React from "react";
import {
  AbsoluteFill,
  interpolate,
  spring,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { PixelSprite } from "../components/PixelSprite";
import { GroundedSprite, HubTilemap } from "../components/GameTilemap";
import { FONTS } from "../styles/theme";
import {
  DiagonalWipeIn,
  ImpactFlash,
  TacticalHudFrame,
} from "../components/CinematicEffects";

export const Scene02_MissionBegins: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const isPart2 = frame >= 75;
  const camScale = interpolate(frame, [0, 150], [1.04, 1.16], {
    extrapolateRight: "clamp",
  });
  const camX = interpolate(frame, [0, 150], [90, -80], {
    extrapolateRight: "clamp",
  });

  const vegaIdx = Math.floor(frame / 4) % 9;
  const vegaFrame = `game-assets/Sprites/Characters/DrElenaVega/Idle/South/vega-idle-south_00${vegaIdx}.png`;

  const bimoIdx = Math.floor(frame / 3) % 4;
  const bimoFrame = `game-assets/Sprites/Characters/BIMO/Move/East/move_east_00${bimoIdx}.png`;

  const mateoIdx = Math.floor(frame / 6) % 2;
  const lunaIdx = Math.floor(frame / 6) % 2;
  const runIdx = Math.floor(frame / 3) % 8;
  const playerFrame = `game-assets/Sprites/Characters/AstronautStocky/Running/East/frame_00${runIdx}.png`;

  const playerX = interpolate(frame, [0, 150], [420, 1080], {
    extrapolateRight: "clamp",
  });
  const bimoX = interpolate(frame, [0, 150], [300, 950], {
    extrapolateRight: "clamp",
  });

  const dialogPop = spring({
    frame: frame - 8,
    fps,
    config: { damping: 14, stiffness: 150 },
  });

  return (
    <AbsoluteFill style={{ backgroundColor: "#0a0f1d", overflow: "hidden" }}>
      <DiagonalWipeIn color="#38bdf8" />
      <ImpactFlash triggerFrame={75} duration={6} color="#38bdf8" maxOpacity={0.45} />

      <AbsoluteFill
        style={{
          transform: `translate(${camX}px, 0px) scale(${camScale})`,
          transformOrigin: "center center",
        }}
      >
        {/* Multi-tile 18-variant Hub Tilemap */}
        <HubTilemap variant="hub" />

        {/* Top Structural Wall with Real Hub Panels & Airlocks */}
        <div
          style={{
            position: "absolute",
            top: 0,
            left: -128,
            right: -128,
            height: 210,
            background: "linear-gradient(180deg, #0b1120 0%, #1e293b 100%)",
            borderBottom: "8px solid #334155",
            display: "flex",
            alignItems: "flex-end",
            justifyContent: "space-around",
            padding: "0 80px",
            zIndex: 10,
          }}
        >
          <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-a.png" size={175} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/esclusa-cian.png" size={200} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-b.png" size={175} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-c.png" size={175} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/esclusa-naranja.png" size={200} />
        </div>

        {/* Grounded Interior Props & Stations with Contact Shadows */}
        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/reactor-cian.png"
          x={240}
          y={310}
          size={210}
        />
        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/servidor.png"
          x={440}
          y={320}
          size={145}
        />
        <GroundedSprite
          src="game-assets/Sprites/Hub/Props/tanques-gemelos.png"
          x={250}
          y={740}
          size={150}
        />

        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/consola-control.png"
          x={780}
          y={320}
          size={200}
        />
        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/consola-investigacion.png"
          x={1030}
          y={320}
          size={200}
        />
        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/mesa-laboratorio.png"
          x={900}
          y={740}
          size={175}
        />

        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/estacion-diagnostico.png"
          x={1370}
          y={320}
          size={200}
        />
        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/cultivo-hidroponico.png"
          x={1630}
          y={330}
          size={185}
        />
        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/consola-logistica.png"
          x={1480}
          y={740}
          size={180}
        />

        {/* Grounded Characters with Y-Sorting & Contact Shadows */}
        <GroundedSprite
          src={`game-assets/Sprites/Characters/Mateo/Idle/South/idle_south_00${mateoIdx}.png`}
          x={520}
          y={420}
          size={135}
          shadowWidth={64}
        />
        <GroundedSprite
          src={vegaFrame}
          x={910}
          y={425}
          size={140}
          shadowWidth={66}
        />
        <GroundedSprite
          src={`game-assets/Sprites/Characters/Luna/Idle/South/idle_south_00${lunaIdx}.png`}
          x={1250}
          y={430}
          size={135}
          shadowWidth={64}
        />

        <GroundedSprite
          src={playerFrame}
          x={playerX}
          y={525}
          size={142}
          shadowWidth={68}
        />
        <GroundedSprite
          src={bimoFrame}
          x={bimoX}
          y={535 + Math.sin(frame * 0.3) * 5}
          size={112}
          shadowWidth={56}
        />
      </AbsoluteFill>

      {/* Top-Left Zone Banner */}
      <div
        style={{
          position: "absolute",
          top: 28,
          left: 40,
          padding: "12px 26px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          border: "3px solid #38bdf8",
          boxShadow: "0 12px 32px rgba(0,0,0,0.85)",
          zIndex: 90,
        }}
      >
        <div style={{ fontFamily: FONTS.title, fontSize: 26, color: "#38bdf8", letterSpacing: 2 }}>
          01_HUB // CENTRO ESPACIAL MARS CRAFTS
        </div>
      </div>

      {/* Bottom Dialogue Box */}
      <div
        style={{
          position: "absolute",
          bottom: 28,
          left: 130,
          right: 130,
          height: 152,
          transform: `scale(${dialogPop})`,
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: "3px solid #38bdf8",
          boxShadow: "0 18px 45px rgba(0,0,0,0.92)",
          display: "flex",
          alignItems: "center",
          padding: "16px 28px",
          gap: 24,
          zIndex: 90,
        }}
      >
        <div
          style={{
            width: 110,
            height: 110,
            background: "#090f20",
            border: "3px solid #38bdf8",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            flexShrink: 0,
          }}
        >
          <PixelSprite
            src={
              !isPart2
                ? "game-assets/Sprites/Portraits/DrElenaVega/vega-portrait-happy.png"
                : "game-assets/Sprites/Portraits/BIMO/bimo_happy.png"
            }
            size={96}
          />
        </div>

        <div style={{ flex: 1 }}>
          <div
            style={{
              fontFamily: FONTS.title,
              fontSize: 23,
              color: !isPart2 ? "#38bdf8" : "#facc15",
              letterSpacing: 1.5,
              marginBottom: 4,
            }}
          >
            {!isPart2
              ? "DRA. ELENA VEGA — BRIEFING DE MISIÓN"
              : "BIMO & MATEO ROJAS — RUMBO AL TALLER DE INGENIERÍA"}
          </div>
          <div style={{ fontFamily: FONTS.body, fontSize: 22, fontWeight: 800, color: "#f8fafc", lineHeight: 1.3 }}>
            {!isPart2
              ? "«Bienvenido, Comandante. Cada decisión de diseño determinará si nuestra misión logra sobrevivir en la Luna y Marte.»"
              : "«¡Esclusa abierta! Vamos a la bahía de ensamblaje para configurar la energía, ruedas e instrumentos del rover.»"}
          </div>
        </div>
      </div>

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
