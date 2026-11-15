import React from "react";
import {
  AbsoluteFill,
  Img,
  interpolate,
  staticFile,
  useCurrentFrame,
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

  // Shot 1 (0..55): Engine Ignition Close-up
  // Shot 2 (55..125): Real Game Launchpad Liftoff (launchpad_background + launch_rocket)
  // Shot 3 (125..210): Seamless 03_Launch Orbital Separation & Interplanetary Transfer (Earth + Satélite Terra Relay -> Moon -> Mars)
  const shot = frame < 55 ? 1 : frame < 125 ? 2 : 3;

  const shakeAmp = shot === 1 ? 13 : shot === 2 ? 8 : 0;
  const shakeX = Math.sin(frame * 1.9) * shakeAmp;
  const shakeY = Math.cos(frame * 2.4) * shakeAmp;

  const rocketY = interpolate(frame, [55, 125], [430, -280], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  const flameIdx = frame % 3;
  const flameSprite = `game-assets/Sprites/Launch/Flame/rocket_flame_00${flameIdx}.png`;

  // In Shot 3, the Cruise Spacecraft travels from Earth (left) past the Moon (center) toward Mars (right)
  const cruiseT = interpolate(frame, [125, 210], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });
  const cruiseX = interpolate(cruiseT, [0, 1], [420, 1380]);
  const cruiseY = interpolate(cruiseT, [0, 0.5, 1], [540, 310, 420]);

  return (
    <AbsoluteFill style={{ backgroundColor: "#02040a", overflow: "hidden" }}>
      <DiagonalWipeIn color="#f97316" />
      <ImpactFlash triggerFrame={0} duration={8} color="#f97316" maxOpacity={0.7} />
      <ImpactFlash triggerFrame={55} duration={8} color="#ffffff" maxOpacity={0.8} />
      <ImpactFlash triggerFrame={125} duration={8} color="#38bdf8" maxOpacity={0.75} />

      {/* SHOT 1: EXTREME BOOSTER ENGINE IGNITION */}
      {shot === 1 && (
        <AbsoluteFill
          style={{
            transform: `translate(${shakeX}px, ${shakeY}px) scale(${interpolate(
              frame,
              [0, 55],
              [1.06, 1.22]
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
          <SpeedLinesOverlay opacity={0.32} color="#facc15" direction="vertical" />
          <ForegroundParticles
            color="#ffffff"
            secondaryColor="#f97316"
            count={36}
            speedY={-14}
            speedX={3}
          />
        </AbsoluteFill>
      )}

      {/* SHOT 2: REAL GAME 03_LAUNCH LIFTOFF */}
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
          <SpeedLinesOverlay opacity={0.24} color="#f97316" direction="vertical" />

          <div style={{ position: "absolute", left: 810, top: rocketY }}>
            <PixelSprite
              src="game-assets/Sprites/Launch/Rocket/launch_rocket.png"
              size={300}
            />
            <div style={{ position: "absolute", left: 82, top: 255 }}>
              <PixelSprite src={flameSprite} size={135} />
            </div>
          </div>

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

      {/* SHOT 3: COHERENT 03_LAUNCH ORBITAL DEPLOYMENT & INTERPLANETARY TRANSFER */}
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

          {/* Trajectory Arcs: Earth Orbit -> Moon -> Mars */}
          <svg
            width="1920"
            height="1080"
            style={{ position: "absolute", inset: 0, pointerEvents: "none" }}
          >
            {/* Earth Orbital Ring for the Terra / Relay Satellite */}
            <circle
              cx="290"
              cy="560"
              r="210"
              fill="none"
              stroke="rgba(56,189,248,0.45)"
              strokeWidth="3"
              strokeDasharray="10 8"
            />
            {/* Transfer Arc to Moon & Mars */}
            <path
              d="M 420 540 Q 900 180 1480 440"
              fill="none"
              stroke="#facc15"
              strokeWidth="4"
              strokeDasharray="14 10"
            />
          </svg>

          {/* Planet Earth on the Left */}
          <div style={{ position: "absolute", left: 150, top: 420 }}>
            <PixelSprite
              src="game-assets/Sprites/Launch/Planets/planet_earth.png"
              size={280}
            />
          </div>

          {/* Satélite Terra / Orbital Relay Stationed in Earth Orbit Linking Telemetry */}
          <div
            style={{
              position: "absolute",
              left: 340,
              top: 280 + Math.sin(frame * 0.18) * 8,
            }}
          >
            <PixelSprite src="cinematic/earth_satellite_sprite.png" size={155} />
          </div>

          {/* Planet Moon in the Center-Top */}
          <div style={{ position: "absolute", left: 860, top: 170 }}>
            <PixelSprite
              src="game-assets/Sprites/Launch/Planets/planet_moon.png"
              size={165}
            />
          </div>

          {/* Planet Mars on the Right */}
          <div style={{ position: "absolute", right: 140, top: 320 }}>
            <PixelSprite
              src="game-assets/Sprites/Launch/Planets/planet_mars.png"
              size={270}
            />
          </div>

          {/* Cruise Spacecraft Traveling Along the Trajectory Toward the Moon & Mars */}
          <div
            style={{
              position: "absolute",
              left: cruiseX,
              top: cruiseY,
              transform: `rotate(${interpolate(cruiseT, [0, 1], [-18, 14])}deg)`,
            }}
          >
            <PixelSprite
              src="game-assets/Sprites/Launch/Spacecraft/cruise_spacecraft.png"
              size={135}
            />
          </div>
        </AbsoluteFill>
      )}

      {/* Top-Left Banner */}
      <div
        style={{
          position: "absolute",
          top: 28,
          left: 40,
          padding: "12px 26px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: `4px solid ${shot === 3 ? "#38bdf8" : "#f97316"}`,
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          zIndex: 90,
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
            : "03_LAUNCH // ÓRBITA TERRESTRE Y TRANSFERENCIA INTERPLANETARIA"}
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
            : "SATÉLITE RELÉ EN ÓRBITA Y CRUCERO TIERRA → LUNA → MARTE"}
        </div>
      </div>

      {/* Bottom Trajectory Telemetry Bar in Shot 3 */}
      {shot === 3 && (
        <div
          style={{
            position: "absolute",
            bottom: 28,
            left: 140,
            right: 140,
            padding: "14px 26px",
            background: "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
            border: "3px solid #38bdf8",
            boxShadow: "0 16px 40px rgba(0,0,0,0.92)",
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            zIndex: 90,
          }}
        >
          <div style={{ display: "flex", alignItems: "center", gap: 16 }}>
            <PixelSprite src="game-assets/Sprites/Icons/Resources/communications.png" size={48} />
            <div>
              <div style={{ fontFamily: FONTS.title, fontSize: 22, color: "#38bdf8" }}>
                ENLACE ORBITAL ESTABLECIDO — INYECCIÓN TRANS-LUNAR Y TRANS-MARCIANA
              </div>
              <div style={{ fontFamily: FONTS.body, fontSize: 19, fontWeight: 800, color: "#f8fafc", marginTop: 2 }}>
                El satélite mantiene la telemetría con la Tierra mientras la cápsula desciende hacia su objetivo planetario.
              </div>
            </div>
          </div>
        </div>
      )}

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
