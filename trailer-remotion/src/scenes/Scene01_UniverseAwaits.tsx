import React from "react";
import {
  AbsoluteFill,
  Img,
  interpolate,
  spring,
  staticFile,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { PixelSprite } from "../components/PixelSprite";
import { FONTS } from "../styles/theme";
import {
  ForegroundParticles,
  ImpactFlash,
  TacticalHudFrame,
} from "../components/CinematicEffects";

// Exact Quadratic Bezier evaluation for P0 -> P1 -> P2
function evalQuadBezier(
  t: number,
  p0: { x: number; y: number },
  p1: { x: number; y: number },
  p2: { x: number; y: number }
) {
  const u = 1 - t;
  const x = u * u * p0.x + 2 * u * t * p1.x + t * t * p2.x;
  const y = u * u * p0.y + 2 * u * t * p1.y + t * t * p2.y;
  const dx = 2 * u * (p1.x - p0.x) + 2 * t * (p2.x - p1.x);
  const dy = 2 * u * (p1.y - p0.y) + 2 * t * (p2.y - p1.y);
  const angleDeg = (Math.atan2(dy, dx) * 180) / Math.PI;
  return { x, y, angleDeg };
}

export const Scene01_UniverseAwaits: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const isShot2 = frame >= 74;

  const camScale = interpolate(frame, [0, 120], [1.02, 1.12], {
    extrapolateRight: "clamp",
  });

  // Well-separated celestial positions:
  // Earth center: (280, 560), radius ~130 -> right orbital rim at (435, 555)
  // Moon center: (620, 225), radius ~70 -> bottom edge at y = 295 (well above the Mars transfer curve!)
  // Mars center: (1620, 455), radius ~125 -> left orbital rim at (1465, 455)
  const p0 = { x: 435, y: 555 };
  const p1 = { x: 960, y: 390 }; // Midpoint of curve is y = 447 (152px below Moon's bottom edge!)
  const p2 = { x: 1465, y: 455 };

  const shipT = interpolate(frame, [0, 74], [0.06, 0.86], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });
  const shipPos = evalQuadBezier(shipT, p0, p1, p2);
  const shipSize = 120;

  // Orbiting Terra Satellite on Earth's LEO ring (cx=280, cy=560, r=165)
  const satAngleRad =
    (interpolate(frame, [0, 74], [-68, -28], { extrapolateRight: "clamp" }) *
      Math.PI) /
    180;
  const satX = 280 + 165 * Math.cos(satAngleRad);
  const satY = 560 + 165 * Math.sin(satAngleRad);

  const quoteOpacity = interpolate(frame, [10, 26, 106, 118], [0, 1, 1, 0], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const missions = [
    {
      card: "game-assets/UI/Missions/Cards/mission_earth.png",
      tag: "MISIÓN 1 // ÓRBITA LEO",
      title: "GUARDIANES DE LA TIERRA",
      sub: "Satélite Terra • Clima y Océanos",
      color: "#38bdf8",
      delay: 76,
    },
    {
      card: "game-assets/UI/Missions/Cards/mission_moon.png",
      tag: "MISIÓN 2 // POLO SUR (-240 °C)",
      title: "EL SECRETO DE LA LUNA",
      sub: "LRO & VIPER • Hielo en Sombra",
      color: "#60a5fa",
      delay: 82,
    },
    {
      card: "game-assets/UI/Missions/Cards/mission_mars.png",
      tag: "MISIÓN 3 // CRÁTER JEZERO",
      title: "MARTE: HUELLAS DEL PASADO",
      sub: "Perseverance • Antiguo Delta",
      color: "#f97316",
      delay: 88,
    },
  ];

  return (
    <AbsoluteFill style={{ backgroundColor: "#030612", overflow: "hidden" }}>
      <ImpactFlash triggerFrame={74} duration={8} color="#38bdf8" maxOpacity={0.75} />

      {/* Real Game Space Background */}
      <AbsoluteFill style={{ transform: `scale(${camScale})` }}>
        <Img
          src={staticFile("game-assets/Sprites/Launch/Backgrounds/space_background.png")}
          style={{
            width: "100%",
            height: "100%",
            objectFit: "cover",
            imageRendering: "pixelated",
          }}
        />

        {/* Sun Corona Glow on Left */}
        <div
          style={{
            position: "absolute",
            left: -220,
            top: 120,
            width: 680,
            height: 680,
            borderRadius: "50%",
            background:
              "radial-gradient(circle, rgba(250,204,21,0.55) 0%, rgba(249,115,22,0.22) 45%, transparent 72%)",
            mixBlendMode: "screen",
          }}
        />

        {/* Exact Geometrically Aligned Trajectory Arcs */}
        <svg
          width="1920"
          height="1080"
          style={{ position: "absolute", inset: 0, pointerEvents: "none" }}
        >
          {/* Earth LEO Orbital Ring */}
          <circle
            cx="280"
            cy="560"
            r="165"
            fill="none"
            stroke="rgba(56,189,248,0.4)"
            strokeWidth="2.5"
            strokeDasharray="8 8"
          />
          {/* Secondary Trans-Lunar Arc (Earth Orbit -> Moon Orbit, cleanly above) */}
          <path
            d="M 350 410 Q 435 265 540 230"
            fill="none"
            stroke="#60a5fa"
            strokeWidth="3.5"
            strokeDasharray="10 10"
            opacity={0.7}
          />
          {/* Main Hohmann Transfer Trajectory (Earth Orbit -> Mars Orbit, sweeping cleanly below the Moon) */}
          <path
            d={`M ${p0.x} ${p0.y} Q ${p1.x} ${p1.y} ${p2.x} ${p2.y}`}
            fill="none"
            stroke="#38bdf8"
            strokeWidth="4.5"
            strokeDasharray="14 12"
            opacity={0.85}
          />
        </svg>

        {/* Planet Earth: center (280, 560), size 250 -> left 155, top 435 */}
        <div
          style={{
            position: "absolute",
            left: 155,
            top: 435,
            filter: "drop-shadow(0 0 35px rgba(56,189,248,0.65))",
          }}
        >
          <PixelSprite
            src="game-assets/Sprites/Launch/Planets/planet_earth.png"
            size={250}
          />
        </div>

        {/* Terra Satellite on Earth's LEO Ring */}
        <div
          style={{
            position: "absolute",
            left: satX - 48,
            top: satY - 48,
          }}
        >
          <PixelSprite src="cinematic/earth_satellite_sprite.png" size={96} />
        </div>

        {/* Planet Moon: center (620, 225), size 140 -> left 550, top 155 */}
        <div
          style={{
            position: "absolute",
            left: 550,
            top: 155,
            filter: "drop-shadow(0 0 25px rgba(148,163,184,0.6))",
          }}
        >
          <PixelSprite
            src="game-assets/Sprites/Launch/Planets/planet_moon.png"
            size={140}
          />
        </div>

        {/* Planet Mars: center (1620, 455), size 250 -> left 1495, top 330 */}
        <div
          style={{
            position: "absolute",
            left: 1495,
            top: 330,
            filter: "drop-shadow(0 0 40px rgba(249,115,22,0.65))",
          }}
        >
          <PixelSprite
            src="game-assets/Sprites/Launch/Planets/planet_mars.png"
            size={250}
          />
        </div>

        {/* Cruise Spacecraft Locked 100% onto the Quadratic Bezier Curve Center & Tangent Angle */}
        <div
          style={{
            position: "absolute",
            left: shipPos.x - shipSize / 2,
            top: shipPos.y - shipSize / 2,
            width: shipSize,
            height: shipSize,
            transform: `rotate(${shipPos.angleDeg}deg)`,
            transformOrigin: "center center",
            filter: "drop-shadow(0 0 18px #facc15)",
          }}
        >
          <PixelSprite
            src="game-assets/Sprites/Launch/Spacecraft/cruise_spacecraft.png"
            size={shipSize}
          />
        </div>
      </AbsoluteFill>

      <ForegroundParticles
        color="#38bdf8"
        secondaryColor="#facc15"
        count={22}
        speedY={-1.5}
        speedX={1.2}
      />

      {/* SHOT 2 OVERLAY: The 3 Unlockable Game Mission Cards (Tierra, Luna, Marte) */}
      {isShot2 && (
        <AbsoluteFill
          style={{
            backgroundColor: "rgba(3, 7, 18, 0.72)",
            display: "flex",
            flexDirection: "row",
            alignItems: "center",
            justifyContent: "center",
            gap: 36,
            paddingBottom: 70,
            zIndex: 55,
          }}
        >
          {missions.map((m, idx) => {
            const pop = spring({
              frame: frame - m.delay,
              fps,
              config: { damping: 12, stiffness: 165 },
            });
            return (
              <div
                key={idx}
                style={{
                  width: 490,
                  padding: "18px 22px",
                  transform: `scale(${pop})`,
                  background:
                    "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
                  imageRendering: "pixelated",
                  border: `4px solid ${m.color}`,
                  boxShadow: `0 18px 45px rgba(0,0,0,0.9), 0 0 30px ${m.color}55`,
                  display: "flex",
                  flexDirection: "column",
                  alignItems: "center",
                  textAlign: "center",
                }}
              >
                <div
                  style={{
                    fontFamily: FONTS.title,
                    fontSize: 18,
                    color: m.color,
                    letterSpacing: 2,
                    marginBottom: 8,
                  }}
                >
                  {m.tag}
                </div>
                <div
                  style={{
                    width: 420,
                    height: 230,
                    overflow: "hidden",
                    border: `3px solid ${m.color}`,
                    marginBottom: 12,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    background: "#020617",
                  }}
                >
                  <Img
                    src={staticFile(m.card)}
                    style={{
                      width: "100%",
                      height: "100%",
                      objectFit: "cover",
                      imageRendering: "pixelated",
                    }}
                  />
                </div>
                <div
                  style={{
                    fontFamily: FONTS.title,
                    fontSize: 28,
                    color: "#ffffff",
                    letterSpacing: 1.5,
                  }}
                >
                  {m.title}
                </div>
                <div
                  style={{
                    fontFamily: FONTS.body,
                    fontSize: 19,
                    fontWeight: 800,
                    color: "#cbd5e1",
                    marginTop: 4,
                  }}
                >
                  {m.sub}
                </div>
              </div>
            );
          })}
        </AbsoluteFill>
      )}

      {/* Conceptual Narrative Quote at Bottom */}
      <div
        style={{
          position: "absolute",
          bottom: 36,
          left: 0,
          right: 0,
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          opacity: quoteOpacity,
          zIndex: 65,
        }}
      >
        <div
          style={{
            padding: "12px 34px",
            background:
              "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
            imageRendering: "pixelated",
            border: "3px solid #38bdf8",
            boxShadow: "0 14px 36px rgba(0,0,0,0.9)",
            textAlign: "center",
          }}
        >
          <div
            style={{
              fontFamily: FONTS.body,
              fontSize: 16,
              fontWeight: 900,
              color: "#38bdf8",
              letterSpacing: 4,
              marginBottom: 4,
            }}
          >
            NASA SPACE APPS CHALLENGE • TRES EXPEDICIONES CIENTÍFICAS
          </div>
          <div
            style={{
              fontFamily: FONTS.title,
              fontSize: 38,
              color: "#ffffff",
              letterSpacing: 2,
            }}
          >
            «Cada gran descubrimiento comienza con una decisión.»
          </div>
        </div>
      </div>

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
