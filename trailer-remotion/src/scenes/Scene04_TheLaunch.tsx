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

export const Scene04_TheLaunch: React.FC = () => {
  const frame = useCurrentFrame();

  // Shot 1 (0..55): Engine Ignition Close-up
  // Shot 2 (55..125): Real Game Launchpad Liftoff (launchpad_background + launch_rocket)
  // Shot 3 (125..210): Seamless 03_Launch Orbital Separation & Interplanetary Transfer (Earth + Satélite Terra Relay -> Moon / Mars)
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

  // SHOT 3 EXACT CELESTIAL & TRAJECTORY GEOMETRY:
  // Earth center: (280, 580), size: 260 (left: 150, top: 450), LEO orbital radius: 185
  // Moon center: (640, 230), size: 145 (left: 568, top: 158) -> bottom edge is y = 302 (well above the Mars transfer curve!)
  // Mars center: (1620, 460), size: 260 (left: 1490, top: 330), capture orbital radius: 165
  // Main Hohmann Transfer Curve P0 -> P1 -> P2:
  // Starts on Earth's right orbital ring (465, 580), sweeps through open space P1=(980, 410) (midpoint y=467, 165px below Moon!),
  // and arrives at Mars's left orbital capture ring P2=(1455, 460).
  const p0 = { x: 465, y: 580 };
  const p1 = { x: 980, y: 410 };
  const p2 = { x: 1455, y: 460 };

  const cruiseT = interpolate(frame, [125, 210], [0.06, 0.92], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });
  const cruisePos = evalQuadBezier(cruiseT, p0, p1, p2);
  const shipSize = 130;

  // Satélite Terra / Orbital Relay orbiting along Earth's r=185 ring
  const satAngleRad =
    (interpolate(frame, [125, 210], [-72, -32], {
      extrapolateLeft: "clamp",
      extrapolateRight: "clamp",
    }) *
      Math.PI) /
    180;
  const satX = 280 + 185 * Math.cos(satAngleRad);
  const satY = 580 + 185 * Math.sin(satAngleRad);
  const satSize = 135;

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
            const spread =
              (frame - 55) * (6.5 + (i % 5) * 3) * (i % 2 === 0 ? 1 : -1);
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

          {/* Geometrically Exact Trajectory Arcs & Telemetry Links */}
          <svg
            width="1920"
            height="1080"
            style={{ position: "absolute", inset: 0, pointerEvents: "none" }}
          >
            {/* Earth Orbital Ring (cx=280, cy=580, r=185) */}
            <circle
              cx="280"
              cy="580"
              r="185"
              fill="none"
              stroke="rgba(56,189,248,0.48)"
              strokeWidth="3"
              strokeDasharray="10 8"
            />
            {/* Lunar Orbital Ring (cx=640, cy=230, r=92) */}
            <circle
              cx="640"
              cy="230"
              r="92"
              fill="none"
              stroke="rgba(96,165,250,0.42)"
              strokeWidth="2.5"
              strokeDasharray="8 8"
            />
            {/* Mars Capture Orbital Ring (cx=1620, cy=460, r=165) */}
            <circle
              cx="1620"
              cy="460"
              r="165"
              fill="none"
              stroke="rgba(249,115,22,0.42)"
              strokeWidth="3"
              strokeDasharray="10 8"
            />

            {/* Trans-Lunar Insertion Arc (Earth Orbit -> Lunar Orbit, cleanly above) */}
            <path
              d="M 365 415 Q 450 270 550 235"
              fill="none"
              stroke="#60a5fa"
              strokeWidth="3.5"
              strokeDasharray="10 10"
              opacity={0.78}
            />

            {/* Main Trans-Mars Hohmann Trajectory (Earth Orbit -> Mars Orbit, sweeping cleanly below the Moon) */}
            <path
              d={`M ${p0.x} ${p0.y} Q ${p1.x} ${p1.y} ${p2.x} ${p2.y}`}
              fill="none"
              stroke="#facc15"
              strokeWidth="4.5"
              strokeDasharray="14 10"
            />

            {/* Active Telemetry Link between Satélite Terra and Cruise Spacecraft */}
            <line
              x1={satX}
              y1={satY}
              x2={cruisePos.x}
              y2={cruisePos.y}
              stroke="#38bdf8"
              strokeWidth="2.5"
              strokeDasharray="6 8"
              opacity={0.65}
            />
          </svg>

          {/* Planet Earth: center (280, 580), size 260 -> left 150, top 450 */}
          <div style={{ position: "absolute", left: 150, top: 450 }}>
            <PixelSprite
              src="game-assets/Sprites/Launch/Planets/planet_earth.png"
              size={260}
            />
          </div>

          {/* Satélite Terra / Orbital Relay Centered Exactly on Earth's r=185 Orbital Ring */}
          <div
            style={{
              position: "absolute",
              left: satX - satSize / 2,
              top: satY - satSize / 2,
              width: satSize,
              height: satSize,
            }}
          >
            <PixelSprite src="cinematic/earth_satellite_sprite.png" size={satSize} />
          </div>

          {/* Planet Moon: center (640, 230), size 145 -> left 568, top 158 */}
          <div style={{ position: "absolute", left: 568, top: 158 }}>
            <PixelSprite
              src="game-assets/Sprites/Launch/Planets/planet_moon.png"
              size={145}
            />
          </div>

          {/* Planet Mars: center (1620, 460), size 260 -> left 1490, top 330 */}
          <div style={{ position: "absolute", left: 1490, top: 330 }}>
            <PixelSprite
              src="game-assets/Sprites/Launch/Planets/planet_mars.png"
              size={260}
            />
          </div>

          {/* Cruise Spacecraft Centered & Tangentially Aligned 100% on the Quadratic Bezier Trajectory */}
          <div
            style={{
              position: "absolute",
              left: cruisePos.x - shipSize / 2,
              top: cruisePos.y - shipSize / 2,
              width: shipSize,
              height: shipSize,
              transform: `rotate(${cruisePos.angleDeg}deg)`,
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
      )}

      {/* Top-Left Banner */}
      <div
        style={{
          position: "absolute",
          top: 28,
          left: 40,
          padding: "12px 26px",
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
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
            : "SATÉLITE RELÉ EN ÓRBITA Y CRUCERO TIERRA → LUNA / MARTE"}
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
            background:
              "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
            border: "3px solid #38bdf8",
            boxShadow: "0 16px 40px rgba(0,0,0,0.92)",
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            zIndex: 90,
          }}
        >
          <div style={{ display: "flex", alignItems: "center", gap: 16 }}>
            <PixelSprite
              src="game-assets/Sprites/Icons/Resources/communications.png"
              size={48}
            />
            <div>
              <div
                style={{
                  fontFamily: FONTS.title,
                  fontSize: 22,
                  color: "#38bdf8",
                }}
              >
                ENLACE ORBITAL ESTABLECIDO — INYECCIÓN TRANS-LUNAR Y TRANS-MARCIANA
              </div>
              <div
                style={{
                  fontFamily: FONTS.body,
                  fontSize: 19,
                  fontWeight: 800,
                  color: "#f8fafc",
                  marginTop: 2,
                }}
              >
                El satélite mantiene la telemetría con la Tierra mientras la cápsula viaja por su trayectoria hacia Marte.
              </div>
            </div>
          </div>
        </div>
      )}

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
