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

const LOADOUT_STEPS = [
  {
    frame: 8,
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
    frame: 44,
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
    frame: 80,
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
    frame: 116,
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
    frame >= 116 ? 3 : frame >= 80 ? 2 : frame >= 44 ? 1 : 0;
  const activeStep = LOADOUT_STEPS[stepIdx];

  // The 3x3 concrete assembly pad in HubTilemap(variant="workshop") is centered at x = 608, y = 544.
  // After frame 135 ("¡LANZANDO!"), the rover rolls out cleanly along the unobstructed grated track (y = 544) from x = 608 to x = 1100!
  const isRollingOut = frame >= 135;
  const roverX = isRollingOut
    ? interpolate(frame, [135, 180], [608, 1100], { extrapolateRight: "clamp" })
    : 608;
  const driveIdx = isRollingOut ? Math.floor(frame / 3) % 4 : 0;

  const camScale = interpolate(frame, [0, 180], [1.04, 1.12], {
    extrapolateRight: "clamp",
  });

  const stampPop = spring({
    frame: frame - 6,
    fps,
    config: { damping: 11, stiffness: 180 },
  });

  return (
    <AbsoluteFill style={{ backgroundColor: "#0a0f1d", overflow: "hidden" }}>
      <DiagonalWipeIn color="#f59e0b" />
      <ImpactFlash triggerFrame={44} duration={5} color="#22c55e" maxOpacity={0.35} />
      <ImpactFlash triggerFrame={80} duration={5} color="#38bdf8" maxOpacity={0.35} />
      <ImpactFlash triggerFrame={116} duration={5} color="#a855f7" maxOpacity={0.4} />
      <ImpactFlash triggerFrame={135} duration={7} color="#facc15" maxOpacity={0.55} />

      {/* COHESIVE TOP-DOWN HANGAR BAY TILEMAP (100% Real Game Tiles & Props — Zero AI Background) */}
      <AbsoluteFill
        style={{
          transform: `scale(${camScale})`,
          transformOrigin: "center center",
        }}
      >
        <HubTilemap variant="workshop" />

        {/* Top Hangar Wall with Orange Launch Bay Airlock */}
        <div
          style={{
            position: "absolute",
            top: 0,
            left: -128,
            right: -128,
            height: 205,
            background: "linear-gradient(180deg, #0b1120 0%, #1e293b 100%)",
            borderBottom: "8px solid #334155",
            display: "flex",
            alignItems: "flex-end",
            justifyContent: "space-around",
            padding: "0 90px",
            zIndex: 10,
          }}
        >
          <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-a.png" size={175} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-b.png" size={175} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/esclusa-naranja.png" size={205} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-c.png" size={175} />
        </div>

        {/* Hangar Engineering Stations, Crates & Tanks Positioned Outside the 3x3 Pad and Rollout Track */}
        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/estacion-diagnostico.png"
          x={350}
          y={305}
          size={195}
        />
        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/mesa-laboratorio.png"
          x={608}
          y={300}
          size={190}
        />
        <GroundedSprite
          src="game-assets/Sprites/Hub/Interior/consola-logistica.png"
          x={880}
          y={305}
          size={190}
        />

        <GroundedSprite
          src="game-assets/Sprites/Hub/Props/contenedor-blanco.png"
          x={260}
          y={510}
          size={140}
        />
        <GroundedSprite
          src="game-assets/Sprites/Hub/Props/caja-naranja.png"
          x={275}
          y={660}
          size={125}
        />
        <GroundedSprite
          src="game-assets/Sprites/Hub/Props/tanques-gemelos.png"
          x={960}
          y={735}
          size={150}
        />

        {/* Mateo Rojas, Commander & BIMO Positioned Around the Perimeter Grate (Clear of the Pad & Rollout Track!) */}
        <GroundedSprite
          src={`game-assets/Sprites/Characters/Mateo/Idle/South/idle_south_00${Math.floor(frame / 6) % 2}.png`}
          x={455}
          y={415}
          size={136}
          shadowWidth={64}
        />
        <GroundedSprite
          src="game-assets/Sprites/Characters/AstronautStocky/Rotations/south.png"
          x={760}
          y={415}
          size={136}
          shadowWidth={64}
        />
        <GroundedSprite
          src={`game-assets/Sprites/Characters/BIMO/Idle/South/idle_south_00${Math.floor(frame / 4) % 4}.png`}
          x={460}
          y={675}
          size={115}
          shadowWidth={56}
        />

        {/* THE ROVER DEAD-CENTER ON THE 3x3 HANGAR PAD (x=608, y=544) */}
        <GroundedSprite
          src={`game-assets/Sprites/Rover/DrivingV2/East/drive_east_00${driveIdx}.png`}
          x={roverX}
          y={544}
          size={205}
          shadowWidth={152}
          shadowHeight={42}
        >
          {/* Centered Module Upgrade Ring on the Floor Around the Rover */}
          <div
            style={{
              position: "absolute",
              left: -18,
              bottom: 6,
              width: 240,
              height: 68,
              borderRadius: "50%",
              border: `3px solid ${activeStep.color}`,
              boxShadow: `0 0 22px ${activeStep.color}88`,
              opacity: 0.85,
            }}
          />
        </GroundedSprite>
      </AbsoluteFill>

      {/* Top-Left Narrative Stamp: DISEÑA */}
      <div
        style={{
          position: "absolute",
          top: 28,
          left: 40,
          transform: `scale(${stampPop})`,
          padding: "12px 26px",
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: "4px solid #f59e0b",
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          zIndex: 90,
        }}
      >
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 50,
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
          02_WORKSHOP // BAHÍA DE ENSAMBLAJE Y RECURSOS
        </div>
      </div>

      {/* Right-Side Authentic Game Workshop Panel */}
      <div
        style={{
          position: "absolute",
          top: 28,
          right: 40,
          bottom: 28,
          width: 620,
          padding: "20px 24px",
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.96) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: "3px solid #38bdf8",
          boxShadow: "0 18px 45px rgba(0,0,0,0.92)",
          display: "flex",
          flexDirection: "column",
          justifyContent: "space-between",
          zIndex: 90,
        }}
      >
        <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
          <div
            style={{
              fontFamily: FONTS.title,
              fontSize: 22,
              color: "#38bdf8",
              letterSpacing: 1.5,
            }}
          >
            CONFIGURACIÓN DEL VEHÍCULO ESPACIAL
          </div>

          <div
            style={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
              <PixelSprite
                src="game-assets/Sprites/Icons/Resources/budget.png"
                size={26}
              />
              <span
                style={{
                  fontFamily: FONTS.title,
                  fontSize: 19,
                  color: "#facc15",
                }}
              >
                PRESUPUESTO: {activeStep.budget} / 160 M$
              </span>
            </div>
            <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
              <PixelSprite
                src="game-assets/Sprites/Icons/Resources/mass.png"
                size={26}
              />
              <span
                style={{
                  fontFamily: FONTS.title,
                  fontSize: 19,
                  color: "#38bdf8",
                }}
              >
                MASA: {activeStep.mass} / 110 kg
              </span>
            </div>
          </div>

          <div
            style={{
              height: 12,
              background: "#090d16",
              border: "2px solid #475569",
            }}
          >
            <div
              style={{
                width: `${(activeStep.mass / 110) * 100}%`,
                height: "100%",
                background: "linear-gradient(90deg, #0284c7, #22c55e)",
              }}
            />
          </div>
        </div>

        <div style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          {LOADOUT_STEPS.map((st, idx) => {
            const isSelected = idx <= stepIdx;
            const isActive = idx === stepIdx;
            return (
              <div
                key={idx}
                style={{
                  padding: "12px 16px",
                  background: isActive
                    ? "rgba(30, 41, 59, 0.95)"
                    : "rgba(15, 23, 42, 0.85)",
                  border: `3px solid ${
                    isActive ? st.color : isSelected ? "#475569" : "#1e293b"
                  }`,
                  opacity: isSelected ? 1 : 0.45,
                  display: "flex",
                  alignItems: "center",
                  gap: 14,
                }}
              >
                <PixelSprite src={st.icon} size={56} />
                <div style={{ flex: 1 }}>
                  <div
                    style={{
                      fontFamily: FONTS.title,
                      fontSize: 14,
                      color: st.color,
                    }}
                  >
                    {st.cat}
                  </div>
                  <div
                    style={{
                      fontFamily: FONTS.title,
                      fontSize: 20,
                      color: "#ffffff",
                    }}
                  >
                    {st.title}
                  </div>
                  <div
                    style={{
                      fontFamily: FONTS.body,
                      fontSize: 15,
                      fontWeight: 800,
                      color: "#cbd5e1",
                    }}
                  >
                    {st.desc}
                  </div>
                </div>
                {isSelected && (
                  <PixelSprite
                    src="game-assets/Sprites/Mars/Indicators/science_completed.png"
                    size={34}
                  />
                )}
              </div>
            );
          })}
        </div>

        <div
          style={{
            padding: "12px 16px",
            background: isRollingOut
              ? "rgba(22, 101, 52, 0.45)"
              : "rgba(15, 23, 42, 0.9)",
            border: `3px solid ${isRollingOut ? "#4ade80" : "#38bdf8"}`,
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
          }}
        >
          <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
            <PixelSprite
              src="game-assets/Sprites/Portraits/BIMO/bimo_happy.png"
              size={48}
            />
            <span
              style={{
                fontFamily: FONTS.body,
                fontSize: 17,
                fontWeight: 800,
                color: "#f8fafc",
              }}
            >
              {isRollingOut
                ? "BIMO: «¡Todo verificado! Saliendo hacia la plataforma.»"
                : "BIMO: «Masa y energía dentro del límite del Lanzador Pesado.»"}
            </span>
          </div>
          <div
            style={{
              padding: "8px 18px",
              background: isRollingOut ? "#22c55e" : "#0284c7",
              fontFamily: FONTS.title,
              fontSize: 19,
              color: "#ffffff",
              flexShrink: 0,
            }}
          >
            {isRollingOut ? "✓ ¡LANZANDO!" : "LANZAR"}
          </div>
        </div>
      </div>

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
