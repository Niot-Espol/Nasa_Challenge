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
  DiagonalWipeIn,
  ForegroundParticles,
  ImpactFlash,
  TacticalHudFrame,
} from "../components/CinematicEffects";

export const Scene08_GrandFinale: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const camScale = interpolate(frame, [0, 150], [1.03, 1.13], {
    extrapolateRight: "clamp",
  });

  const logoPop = spring({
    frame: frame - 8,
    fps,
    config: { damping: 12, stiffness: 135 },
  });

  const badges = [
    {
      title: "MISIÓN 1: TIERRA",
      sub: "Observación Orbital",
      icon: "game-assets/UI/Rewards/Extra/badge_efficient.png",
      color: "#38bdf8",
    },
    {
      title: "MISIÓN 2: LUNA",
      sub: "Hielo Polar (-240 °C)",
      icon: "game-assets/UI/Rewards/Extra/badge_no_safe_mode.png",
      color: "#60a5fa",
    },
    {
      title: "MISIÓN 3: MARTE",
      sub: "Comandante Jezero",
      icon: "game-assets/UI/Rewards/badge_commander.png",
      color: "#f59e0b",
    },
  ];

  return (
    <AbsoluteFill style={{ backgroundColor: "#04060e", overflow: "hidden" }}>
      <DiagonalWipeIn color="#facc15" />
      <ImpactFlash triggerFrame={0} duration={12} color="#facc15" maxOpacity={0.7} />

      <AbsoluteFill style={{ transform: `scale(${camScale})` }}>
        <Img
          src={staticFile("cinematic/finale_commander_mars_horizon.png")}
          style={{
            width: "100%",
            height: "100%",
            objectFit: "cover",
            imageRendering: "pixelated",
          }}
        />
        <AbsoluteFill
          style={{
            background:
              "radial-gradient(circle at 50% 45%, rgba(4,8,20,0.35) 0%, rgba(2,6,18,0.75) 100%)",
          }}
        />
      </AbsoluteFill>

      <ForegroundParticles
        color="#facc15"
        secondaryColor="#38bdf8"
        count={24}
        speedY={-1.8}
        speedX={1.0}
      />

      {/* Central Official Logo, 3 Stars, Tagline & 3 Official Mission Badges */}
      <AbsoluteFill
        style={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          zIndex: 65,
        }}
      >
        <div style={{ display: "flex", gap: 22, marginBottom: 10 }}>
          {[0, 1, 2].map((i) => {
            const sPop = spring({
              frame: frame - (6 + i * 7),
              fps,
              config: { damping: 10, stiffness: 170 },
            });
            return (
              <div key={i} style={{ transform: `scale(${sPop})` }}>
                <PixelSprite src="game-assets/UI/Rewards/Stars/star_full.png" size={74} />
              </div>
            );
          })}
        </div>

        <div
          style={{
            transform: `scale(${logoPop})`,
            filter: "drop-shadow(0 16px 36px rgba(0,0,0,0.95))",
          }}
        >
          <PixelSprite src="game-assets/UI/Title/mars_crafts_logo.png" size={370} />
        </div>

        <div
          style={{
            marginTop: 8,
            fontFamily: FONTS.title,
            fontSize: 40,
            color: "#ffffff",
            letterSpacing: 4,
            textShadow: "0 4px 20px rgba(0,0,0,0.95)",
          }}
        >
          DISEÑA • EXPLORA • DESCUBRE • RESUELVE
        </div>

        <div
          style={{
            marginTop: 12,
            padding: "10px 30px",
            background: "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
            border: "3px solid #facc15",
            fontFamily: FONTS.body,
            fontSize: 24,
            fontWeight: 900,
            color: "#fef08a",
            letterSpacing: 1.5,
          }}
        >
          «Cada gran descubrimiento comienza con una decisión.»
        </div>

        <div style={{ display: "flex", gap: 22, marginTop: 22 }}>
          {badges.map((b, idx) => {
            const bPop = spring({
              frame: frame - (24 + idx * 8),
              fps,
              config: { damping: 12, stiffness: 150 },
            });
            return (
              <div
                key={idx}
                style={{
                  transform: `scale(${bPop})`,
                  padding: "10px 22px",
                  background: "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
                  border: `3px solid ${b.color}`,
                  boxShadow: "0 12px 30px rgba(0,0,0,0.85)",
                  display: "flex",
                  alignItems: "center",
                  gap: 14,
                }}
              >
                <PixelSprite src={b.icon} size={56} />
                <div>
                  <div style={{ fontFamily: FONTS.title, fontSize: 20, color: b.color }}>
                    {b.title}
                  </div>
                  <div style={{ fontFamily: FONTS.body, fontSize: 16, fontWeight: 800, color: "#f8fafc" }}>
                    {b.sub}
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </AbsoluteFill>

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
