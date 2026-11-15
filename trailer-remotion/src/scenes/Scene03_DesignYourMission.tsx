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

const LOADOUT_STEPS = [
  {
    frame: 10,
    cat: "ENERGÍA",
    title: "Generador RTG",
    desc: "+2.5 E/s constante en tormenta y sombra",
    icon: "game-assets/Sprites/Icons/Components/rtg_generator.png",
    budget: 95,
    mass: 68,
    energy: "+2.5/s",
    color: "#f59e0b",
  },
  {
    frame: 48,
    cat: "LOCOMOCIÓN",
    title: "Ruedas Todoterreno",
    desc: "Cruza zonas rocosas y cráteres sin daño",
    icon: "game-assets/Sprites/Icons/Components/wheels_all_terrain.png",
    budget: 115,
    mass: 84,
    energy: "+2.5/s",
    color: "#22c55e",
  },
  {
    frame: 86,
    cat: "COMUNICACIÓN",
    title: "Relé Vía Orbitador",
    desc: "Mantiene señal en cráter lunar y tormenta",
    icon: "game-assets/Sprites/Icons/Components/orbital_relay.png",
    budget: 130,
    mass: 94,
    energy: "+2.5/s",
    color: "#38bdf8",
  },
  {
    frame: 122,
    cat: "INSTRUMENTOS + LANZADOR",
    title: "Taladro + Espectrómetro + Pesado",
    desc: "Capacidad 110 kg • 3/3 instrumentos listos",
    icon: "game-assets/Sprites/Icons/Components/drill_arm.png",
    budget: 145,
    mass: 106,
    energy: "+2.5/s",
    color: "#a855f7",
  },
];

export const Scene03_DesignYourMission: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const stepIdx =
    frame >= 122 ? 3 : frame >= 86 ? 2 : frame >= 48 ? 1 : 0;
  const activeStep = LOADOUT_STEPS[stepIdx];

  const camScale = interpolate(frame, [0, 180], [1.02, 1.1], {
    extrapolateRight: "clamp",
  });

  const roverBounce = Math.sin(frame * 0.25) * 4;
  const stampPop = spring({
    frame: frame - 8,
    fps,
    config: { damping: 11, stiffness: 180 },
  });

  const launchBtnPop = spring({
    frame: frame - 135,
    fps,
    config: { damping: 10, stiffness: 170 },
  });

  return (
    <AbsoluteFill style={{ backgroundColor: "#090d18", overflow: "hidden" }}>
      <DiagonalWipeIn color="#f59e0b" />
      <ImpactFlash triggerFrame={48} duration={6} color="#22c55e" maxOpacity={0.5} />
      <ImpactFlash triggerFrame={86} duration={6} color="#38bdf8" maxOpacity={0.5} />
      <ImpactFlash triggerFrame={122} duration={8} color="#facc15" maxOpacity={0.65} />

      {/* REAL GAME SCENARIO: 02_Workshop Background (UI/Workshop/workshop_background.png) */}
      <AbsoluteFill style={{ transform: `scale(${camScale})` }}>
        <Img
          src={staticFile("game-assets/UI/Workshop/workshop_background.png")}
          style={{
            width: "100%",
            height: "100%",
            objectFit: "cover",
            imageRendering: "pixelated",
          }}
        />

        {/* Subtle Dark Gradient only around edges so the real Workshop art shines in the center */}
        <AbsoluteFill
          style={{
            background:
              "radial-gradient(circle at 50% 48%, transparent 35%, rgba(5,10,24,0.65) 100%)",
          }}
        />

        {/* CENTER WORKBENCH STAGE: Real Game Rover (rover_side.png) + Terra Satellite + Welding Sparks */}
        <div
          style={{
            position: "absolute",
            left: 520,
            top: 230 + roverBounce,
            width: 880,
            height: 460,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          {/* Glowing Blueprint Grid Ring under the Rover */}
          <div
            style={{
              position: "absolute",
              bottom: 35,
              width: 680,
              height: 110,
              borderRadius: "50%",
              background:
                "radial-gradient(ellipse, rgba(56,189,248,0.45) 0%, rgba(56,189,248,0.12) 60%, transparent 75%)",
              border: "3px solid rgba(56,189,248,0.6)",
            }}
          />

          {/* Real Game Workshop Rover Sprite (rover_side.png) */}
          <div
            style={{
              filter: `drop-shadow(0 0 28px ${activeStep.color}88)`,
              transform: `scale(${interpolate(
                spring({ frame: frame - activeStep.frame, fps, config: { damping: 10, stiffness: 180 } }),
                [0, 0.5, 1],
                [0.94, 1.06, 1]
              )})`,
            }}
          >
            <PixelSprite
              src="game-assets/Sprites/Rover/Workshop/rover_side.png"
              size={430}
            />
          </div>

          {/* Floating Installed Component Hologram Nodes Around the Rover */}
          <div style={{ position: "absolute", left: 70, top: 40 }}>
            <div
              style={{
                padding: "10px 14px",
                background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
                border: "3px solid #38bdf8",
                display: "flex",
                alignItems: "center",
                gap: 10,
              }}
            >
              <PixelSprite src="cinematic/earth_satellite_sprite.png" size={56} />
              <div>
                <div style={{ fontFamily: FONTS.title, fontSize: 15, color: "#38bdf8" }}>
                  MISIÓN 1 // ÓRBITA
                </div>
                <div style={{ fontFamily: FONTS.title, fontSize: 18, color: "#ffffff" }}>
                  Satélite Terra
                </div>
              </div>
            </div>
          </div>

          <div style={{ position: "absolute", right: 55, top: 40 }}>
            <div
              style={{
                padding: "10px 14px",
                background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
                border: "3px solid #f59e0b",
                display: "flex",
                alignItems: "center",
                gap: 10,
              }}
            >
              <PixelSprite
                src="game-assets/Sprites/Icons/Components/launcher_heavy.png"
                size={56}
              />
              <div>
                <div style={{ fontFamily: FONTS.title, fontSize: 15, color: "#facc15" }}>
                  LANZADOR PESADO
                </div>
                <div style={{ fontFamily: FONTS.title, fontSize: 18, color: "#ffffff" }}>
                  Capacidad 110 kg
                </div>
              </div>
            </div>
          </div>

          {/* Vertical Laser Verification Scanner sweeping across the Rover */}
          <div
            style={{
              position: "absolute",
              left: 160 + ((frame * 11) % 560),
              top: 30,
              width: 6,
              height: 390,
              background:
                "linear-gradient(180deg, transparent 0%, #38bdf8 20%, #ffffff 50%, #38bdf8 80%, transparent 100%)",
              boxShadow: "0 0 24px #38bdf8",
            }}
          />
        </div>

        {/* Engineers Mateo Rojas & BIMO Physically Working in the Workshop */}
        <div style={{ position: "absolute", left: 250, top: 470 }}>
          <PixelSprite
            src={`game-assets/Sprites/Characters/Mateo/Idle/South/idle_south_00${Math.floor(frame / 6) % 2}.png`}
            size={155}
          />
        </div>
        <div
          style={{
            position: "absolute",
            right: 260,
            top: 485 + Math.sin(frame * 0.3) * 8,
          }}
        >
          <PixelSprite
            src={`game-assets/Sprites/Characters/BIMO/Idle/South/idle_south_00${Math.floor(frame / 4) % 4}.png`}
            size={135}
          />
        </div>
      </AbsoluteFill>

      <ForegroundParticles
        color="#facc15"
        secondaryColor="#38bdf8"
        count={28}
        speedY={-4.5}
        speedX={2.2}
      />

      {/* Top-Left Narrative Stamp: DISEÑA */}
      <div
        style={{
          position: "absolute",
          top: 28,
          left: 40,
          transform: `scale(${stampPop})`,
          padding: "12px 26px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: "4px solid #f59e0b",
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          zIndex: 65,
        }}
      >
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 52,
            color: "#facc15",
            letterSpacing: 5,
            lineHeight: 1,
          }}
        >
          DISEÑA
        </div>
        <div
          style={{
            fontFamily: FONTS.body,
            fontSize: 17,
            fontWeight: 900,
            color: "#f8fafc",
            letterSpacing: 1.5,
            marginTop: 4,
          }}
        >
          02_WORKSHOP // DECISIONES DE INGENIERÍA Y RECURSOS
        </div>
      </div>

      {/* Top-Right Authentic Game Resource Bars (resource_bar_frame.png + budget.png + mass.png + energy.png) */}
      <div
        style={{
          position: "absolute",
          top: 28,
          right: 40,
          width: 520,
          padding: "14px 22px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: "3px solid #38bdf8",
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          display: "flex",
          flexDirection: "column",
          gap: 10,
          zIndex: 65,
        }}
      >
        <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
          <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
            <PixelSprite src="game-assets/Sprites/Icons/Resources/budget.png" size={28} />
            <span style={{ fontFamily: FONTS.title, fontSize: 20, color: "#facc15" }}>
              PRESUPUESTO: {activeStep.budget} / 160 M$
            </span>
          </div>
          <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
            <PixelSprite src="game-assets/Sprites/Icons/Resources/energy.png" size={28} />
            <span style={{ fontFamily: FONTS.title, fontSize: 20, color: "#4ade80" }}>
              {activeStep.energy}
            </span>
          </div>
        </div>

        <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
          <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
            <PixelSprite src="game-assets/Sprites/Icons/Resources/mass.png" size={28} />
            <span style={{ fontFamily: FONTS.title, fontSize: 20, color: "#38bdf8" }}>
              MASA DEL ROVER: {activeStep.mass} / 110 kg
            </span>
          </div>
          {frame >= 130 && (
            <div
              style={{
                transform: `scale(${launchBtnPop})`,
                padding: "6px 16px",
                backgroundImage: `url(${staticFile("game-assets/UI/Buttons/button_hover.png")})`,
                backgroundSize: "100% 100%",
                border: "2px solid #4ade80",
                fontFamily: FONTS.title,
                fontSize: 18,
                color: "#4ade80",
              }}
            >
              ✓ ¡LANZAR MISIÓN!
            </div>
          )}
        </div>
      </div>

      {/* Bottom Compact Component Cards Strip using Real Game UI/Panels/panel_frame.png */}
      <div
        style={{
          position: "absolute",
          bottom: 28,
          left: 44,
          right: 44,
          display: "flex",
          gap: 16,
          zIndex: 65,
        }}
      >
        {LOADOUT_STEPS.map((st, idx) => {
          const isSelected = idx <= stepIdx;
          const isActive = idx === stepIdx;
          return (
            <div
              key={idx}
              style={{
                flex: 1,
                padding: "14px 16px",
                background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
                imageRendering: "pixelated",
                border: `3px solid ${isActive ? st.color : isSelected ? "#64748b" : "#1e293b"}`,
                opacity: isSelected ? 1 : 0.48,
                transform: `scale(${isActive ? 1.03 : 0.98})`,
                boxShadow: isActive
                  ? `0 0 28px ${st.color}66`
                  : "0 10px 24px rgba(0,0,0,0.8)",
                display: "flex",
                alignItems: "center",
                gap: 14,
              }}
            >
              <PixelSprite src={st.icon} size={64} />
              <div>
                <div style={{ fontFamily: FONTS.title, fontSize: 15, color: st.color }}>
                  {st.cat}
                </div>
                <div style={{ fontFamily: FONTS.title, fontSize: 21, color: "#ffffff", marginTop: 2 }}>
                  {st.title}
                </div>
                <div style={{ fontFamily: FONTS.body, fontSize: 15, fontWeight: 800, color: "#cbd5e1", marginTop: 2 }}>
                  {st.desc}
                </div>
              </div>
            </div>
          );
        })}
      </div>

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
