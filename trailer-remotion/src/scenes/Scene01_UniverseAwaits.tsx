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

export const Scene01_UniverseAwaits: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  // Shot 1 (0..74): Real Space Scenario (space_background + planet_earth + planet_moon + planet_mars + cruise_spacecraft)
  // Shot 2 (74..120): The 3 Official Game Mission Cards (mission_earth.png, mission_moon.png, mission_mars.png) unlocking!
  const isShot2 = frame >= 74;

  const camScale = interpolate(frame, [0, 120], [1.02, 1.14], {
    extrapolateRight: "clamp",
  });

  const shipX = interpolate(frame, [0, 74], [360, 1280], {
    extrapolateRight: "clamp",
  });
  const shipY = interpolate(frame, [0, 37, 74], [520, 350, 440], {
    extrapolateRight: "clamp",
  });

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

        {/* Real Game Planets: Earth, Moon, Mars */}
        <div
          style={{
            position: "absolute",
            left: 190,
            top: 360 + Math.sin(frame * 0.05) * 8,
            filter: "drop-shadow(0 0 35px rgba(56,189,248,0.65))",
          }}
        >
          <PixelSprite
            src="game-assets/Sprites/Launch/Planets/planet_earth.png"
            size={260}
          />
        </div>

        <div
          style={{
            position: "absolute",
            left: 820,
            top: 220 + Math.cos(frame * 0.06) * 8,
            filter: "drop-shadow(0 0 25px rgba(148,163,184,0.6))",
          }}
        >
          <PixelSprite
            src="game-assets/Sprites/Launch/Planets/planet_moon.png"
            size={155}
          />
        </div>

        <div
          style={{
            position: "absolute",
            right: 190,
            top: 320 + Math.sin(frame * 0.05 + 1) * 8,
            filter: "drop-shadow(0 0 40px rgba(249,115,22,0.65))",
          }}
        >
          <PixelSprite
            src="game-assets/Sprites/Launch/Planets/planet_mars.png"
            size={250}
          />
        </div>

        {/* Dotted Transfer Trajectory Arc */}
        <svg
          width="1920"
          height="1080"
          style={{ position: "absolute", inset: 0, pointerEvents: "none" }}
        >
          <path
            d="M 340 480 Q 880 180 1580 430"
            fill="none"
            stroke="#38bdf8"
            strokeWidth="4"
            strokeDasharray="14 12"
            opacity={0.75}
          />
        </svg>

        {/* Real Game Cruise Spacecraft */}
        <div
          style={{
            position: "absolute",
            left: shipX,
            top: shipY,
            transform: "rotate(-12deg)",
            filter: "drop-shadow(0 0 18px #facc15)",
          }}
        >
          <PixelSprite
            src="game-assets/Sprites/Launch/Spacecraft/cruise_spacecraft.png"
            size={125}
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
                  background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
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
            background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
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
