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

  const camScale = interpolate(frame, [0, 150], [1.03, 1.14], {
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
      sub: "Satélite Terra (LEO)",
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
      <ImpactFlash triggerFrame={0} duration={12} color="#facc15" maxOpacity={0.75} />

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
      </AbsoluteFill>

      <ForegroundParticles
        color="#facc15"
        secondaryColor="#38bdf8"
        count={28}
        speedY={-2}
        speedX={1.2}
      />

      {/* Real Game Rover & Crew on the Horizon */}
      <div style={{ position: "absolute", left: 120, bottom: 55 }}>
        <PixelSprite
          src="game-assets/Sprites/Rover/DrivingV2/East/drive_east_000.png"
          size={215}
        />
      </div>
      <div style={{ position: "absolute", right: 350, bottom: 65 }}>
        <PixelSprite
          src="game-assets/Sprites/Characters/DrElenaVega/Rotations/vega-idle-west.png"
          size={135}
        />
      </div>
      <div style={{ position: "absolute", right: 220, bottom: 65 }}>
        <PixelSprite
          src="game-assets/Sprites/Characters/AstronautStocky/Rotations/south.png"
          size={140}
        />
      </div>
      <div
        style={{
          position: "absolute",
          right: 110,
          bottom: 75 + Math.sin(frame * 0.22) * 8,
        }}
      >
        <PixelSprite
          src="game-assets/Sprites/Characters/BIMO/Idle/South/idle_south_000.png"
          size={110}
        />
      </div>

      {/* Central Official Logo, 3 Stars, Tagline & 3 Mission Badges */}
      <AbsoluteFill
        style={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          paddingBottom: 40,
          zIndex: 65,
        }}
      >
        {/* 3 Real Game Golden Stars (UI/Rewards/Stars/star_full.png) */}
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

        {/* Official Game Logo (UI/Title/mars_crafts_logo.png) */}
        <div
          style={{
            transform: `scale(${logoPop})`,
            filter: "drop-shadow(0 16px 36px rgba(0,0,0,0.95))",
          }}
        >
          <PixelSprite src="game-assets/UI/Title/mars_crafts_logo.png" size={360} />
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
            marginTop: 10,
            padding: "10px 30px",
            background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
            imageRendering: "pixelated",
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

        {/* 3 Official Game Rewards & Campaign Missions */}
        <div style={{ display: "flex", gap: 22, marginTop: 20 }}>
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
                  padding: "10px 20px",
                  background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
                  imageRendering: "pixelated",
                  border: `3px solid ${b.color}`,
                  boxShadow: "0 12px 30px rgba(0,0,0,0.85)",
                  display: "flex",
                  alignItems: "center",
                  gap: 14,
                }}
              >
                <PixelSprite src={b.icon} size={58} />
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
