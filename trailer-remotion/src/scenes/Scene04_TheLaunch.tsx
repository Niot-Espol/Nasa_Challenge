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
  SpeedLinesOverlay,
  TacticalHudFrame,
} from "../components/CinematicEffects";

export const Scene04_TheLaunch: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  // Shot 1 (0..55): Extreme Engine Ignition Close-up (the shot the user loved!)
  // Shot 2 (55..130): REAL GAME SCENARIO 03_Launch (launchpad_background.png + launch_rocket.png + rocket_flame + launch_smoke)
  // Shot 3 (130..210): MISSION 1 — GUARDIANS OF EARTH: Real space_background.png + planet_earth.png + earth_satellite_sprite.png scanning Earth!
  const shot = frame < 55 ? 1 : frame < 130 ? 2 : 3;

  const shakeAmp = shot === 1 ? 13 : shot === 2 ? 8 : 1.5;
  const shakeX = Math.sin(frame * 1.9) * shakeAmp;
  const shakeY = Math.cos(frame * 2.4) * shakeAmp;

  const rocketY = interpolate(frame, [55, 130], [430, -280], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const flameIdx = frame % 3;
  const flameSprite = `game-assets/Sprites/Launch/Flame/rocket_flame_00${flameIdx}.png`;

  const satX = interpolate(frame, [130, 210], [520, 980], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  return (
    <AbsoluteFill style={{ backgroundColor: "#02040a", overflow: "hidden" }}>
      <DiagonalWipeIn color="#f97316" />
      <ImpactFlash triggerFrame={0} duration={8} color="#f97316" maxOpacity={0.75} />
      <ImpactFlash triggerFrame={55} duration={8} color="#ffffff" maxOpacity={0.85} />
      <ImpactFlash triggerFrame={130} duration={9} color="#38bdf8" maxOpacity={0.8} />

      {/* SHOT 1: EXTREME BOOSTER ENGINE IGNITION */}
      {shot === 1 && (
        <AbsoluteFill
          style={{
            transform: `translate(${shakeX}px, ${shakeY}px) scale(${interpolate(
              frame,
              [0, 55],
              [1.06, 1.24]
            )})`,
          }}
        >
          <Img
            src={staticFile("cinematic/rocket_engine_ignition_closeup.png")}
            style={{
              width: "100%",
              height: "100%",
              objectFit: "cover",
              imageRendering: "pixelated",
            }}
          />
          <SpeedLinesOverlay opacity={0.35} color="#facc15" direction="vertical" />
          <ForegroundParticles
            color="#ffffff"
            secondaryColor="#f97316"
            count={38}
            speedY={-14}
            speedX={3}
          />
        </AbsoluteFill>
      )}

      {/* SHOT 2: REAL GAME SCENARIO 03_LAUNCH (launchpad_background.png + launch_rocket.png) */}
      {shot === 2 && (
        <AbsoluteFill
          style={{
            transform: `translate(${shakeX}px, ${shakeY}px) scale(1.05)`,
          }}
        >
          <Img
            src={staticFile("game-assets/Sprites/Launch/Backgrounds/launchpad_background.png")}
            style={{
              width: "100%",
              height: "100%",
              objectFit: "cover",
              imageRendering: "pixelated",
            }}
          />
          <SpeedLinesOverlay opacity={0.26} color="#f97316" direction="vertical" />

          {/* Ascending Real Game Rocket + Animated Flame */}
          <div style={{ position: "absolute", left: 810, top: rocketY }}>
            <PixelSprite
              src="game-assets/Sprites/Launch/Rocket/launch_rocket.png"
              size={300}
            />
            <div style={{ position: "absolute", left: 82, top: 255 }}>
              <PixelSprite src={flameSprite} size={135} />
            </div>
          </div>

          {/* Expanding Real Game Smoke Clouds (launch_smoke_000..003.png) */}
          {Array.from({ length: 14 }, (_, i) => {
            const spread = (frame - 55) * (6.5 + (i % 5) * 3) * (i % 2 === 0 ? 1 : -1);
            const lift = (frame - 55) * (2.2 + (i % 4));
            const smokeIdx = (i + Math.floor(frame / 4)) % 4;
            return (
              <div
                key={i}
                style={{
                  position: "absolute",
                  left: 880 + spread,
                  bottom: 45 + lift,
                  opacity: 0.88,
                }}
              >
                <PixelSprite
                  src={`game-assets/Sprites/Launch/Smoke/launch_smoke_00${smokeIdx}.png`}
                  size={150 + (i % 3) * 45}
                />
              </div>
            );
          })}
        </AbsoluteFill>
      )}

      {/* SHOT 3: MISSION 1 — GUARDIANS OF EARTH (REAL GAME SPACE + PLANET EARTH + TERRA SATELLITE) */}
      {shot === 3 && (
        <AbsoluteFill>
          <Img
            src={staticFile("game-assets/Sprites/Launch/Backgrounds/space_background.png")}
            style={{
              width: "100%",
              height: "100%",
              objectFit: "cover",
              imageRendering: "pixelated",
            }}
          />

          {/* Giant Real Game Planet Earth Below in Low Earth Orbit */}
          <div
            style={{
              position: "absolute",
              left: 420,
              bottom: -420,
              filter: "drop-shadow(0 -20px 80px rgba(56,189,248,0.75))",
            }}
          >
            <PixelSprite
              src="game-assets/Sprites/Launch/Planets/planet_earth.png"
              size={1080}
            />
          </div>

          {/* Cyan Multispectral Observation Beam Scanning Earth's Oceans & Atmosphere */}
          <div
            style={{
              position: "absolute",
              left: satX - 90,
              top: 290,
              width: 420,
              height: 520,
              background:
                "linear-gradient(180deg, rgba(56,189,248,0.75) 0%, rgba(34,211,238,0.28) 65%, transparent 100%)",
              clipPath: "polygon(42% 0%, 58% 0%, 100% 100%, 0% 100%)",
              mixBlendMode: "screen",
            }}
          />

          {/* Orbiting NASA Terra Satellite Sprite */}
          <div
            style={{
              position: "absolute",
              left: satX,
              top: 150 + Math.sin(frame * 0.2) * 10,
              filter: "drop-shadow(0 0 28px #38bdf8)",
            }}
          >
            <PixelSprite src="cinematic/earth_satellite_sprite.png" size={235} />
          </div>

          {/* Picture-in-Picture Cinematic View of the Satellite Fairing Deployment */}
          <div
            style={{
              position: "absolute",
              right: 48,
              top: 150,
              width: 460,
              height: 260,
              padding: 8,
              background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
              border: "3px solid #38bdf8",
              boxShadow: "0 16px 40px rgba(0,0,0,0.9)",
            }}
          >
            <Img
              src={staticFile("cinematic/earth_mission_satellite_deploy.png")}
              style={{
                width: "100%",
                height: "100%",
                objectFit: "cover",
                imageRendering: "pixelated",
              }}
            />
          </div>
        </AbsoluteFill>
      )}

      {/* Top-Left Banner using Real Game UI/Panels/panel_frame.png */}
      <div
        style={{
          position: "absolute",
          top: 30,
          left: 42,
          padding: "12px 26px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: `4px solid ${shot === 3 ? "#38bdf8" : "#f97316"}`,
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          zIndex: 65,
        }}
      >
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 18,
            color: shot === 3 ? "#38bdf8" : "#fdba74",
            letterSpacing: 2,
          }}
        >
          {shot === 1
            ? "03_LAUNCH // IGNICIÓN DE MOTORES"
            : shot === 2
            ? "03_LAUNCH // ASCENSO DESDE PLATAFORMA"
            : "MISIÓN 1 // GUARDIANES DE LA TIERRA (SATÉLITE NASA TERRA)"}
        </div>
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 32,
            color: "#ffffff",
            letterSpacing: 1.5,
            marginTop: 4,
          }}
        >
          {shot === 1
            ? "¡EMPUJE AL 108 % — IGNICIÓN PRINCIPAL!"
            : shot === 2
            ? "¡DESPEGUE RUMBO AL SISTEMA SOLAR!"
            : "ESCANEO ORBITAL DE CLIMA, OCÉANOS Y VEGETACIÓN"}
        </div>
      </div>

      {/* Bottom NASA Terra Telemetry Popup in Shot 3 */}
      {shot === 3 && (
        <div
          style={{
            position: "absolute",
            bottom: 30,
            left: 130,
            right: 130,
            padding: "16px 28px",
            background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
            imageRendering: "pixelated",
            border: "3px solid #38bdf8",
            boxShadow: "0 16px 40px rgba(0,0,0,0.92)",
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            zIndex: 65,
          }}
        >
          <div style={{ display: "flex", alignItems: "center", gap: 18 }}>
            <PixelSprite src="game-assets/Sprites/Icons/Resources/science_data.png" size={58} />
            <div>
              <div style={{ fontFamily: FONTS.title, fontSize: 22, color: "#38bdf8" }}>
                MISIÓN 1 COMPLETADA — OBSERVATORIO TERRESTRE NASA TERRA (LEO)
              </div>
              <div style={{ fontFamily: FONTS.body, fontSize: 20, fontWeight: 800, color: "#f8fafc", marginTop: 3 }}>
                Datos atmosféricos y oceánicos transmitidos al Centro Espacial. ¡Misión 2 (La Luna) desbloqueada!
              </div>
            </div>
          </div>
          <div
            style={{
              padding: "10px 20px",
              background: "rgba(34,197,94,0.22)",
              border: "3px solid #22c55e",
              fontFamily: FONTS.title,
              fontSize: 22,
              color: "#4ade80",
              flexShrink: 0,
            }}
          >
            DATOS TERRA: 3 / 3
          </div>
        </div>
      )}

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
