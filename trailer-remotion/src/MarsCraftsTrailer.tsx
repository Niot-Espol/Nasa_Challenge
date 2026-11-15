import React from 'react';
import { AbsoluteFill, Audio, Sequence, staticFile } from 'remotion';
import { Scene01_UniverseAwaits } from './scenes/Scene01_UniverseAwaits';
import { Scene02_MissionBegins } from './scenes/Scene02_MissionBegins';
import { Scene03_DesignYourMission } from './scenes/Scene03_DesignYourMission';
import { Scene04_TheLaunch } from './scenes/Scene04_TheLaunch';
import { Scene05_ExploreMars } from './scenes/Scene05_ExploreMars';
import { Scene06_MoonSecret } from './scenes/Scene06_MoonSecret';
import { Scene07_ControlRescue } from './scenes/Scene07_ControlRescue';
import { Scene08_GrandFinale } from './scenes/Scene08_GrandFinale';
import { ensureTrailerFonts } from './styles/theme';

export const MarsCraftsTrailer: React.FC = () => {
  ensureTrailerFonts();

  return (
    <AbsoluteFill style={{ backgroundColor: '#05070D' }}>
      {/* Banda sonora original 16-bit Space Adventure (45.0s exactos, 120 BPM, estéreo) */}
      <Audio src={staticFile('audio/mars_crafts_trailer_ost.mp3')} volume={0.88} />

      {/* Efectos de sonido reales del juego sincronizados al fotograma */}
      {/* Escena 1: Aparición de las 3 tarjetas de misión */}
      <Sequence from={74} durationInFrames={25} name="SFX_Mission_Cards">
        <Audio src={staticFile('audio/star_ding.wav')} volume={0.45} />
      </Sequence>

      {/* Escena 2: Diálogo en el Hub y apertura de esclusa al Taller */}
      <Sequence from={128} durationInFrames={25} name="SFX_Hub_Dialogue">
        <Audio src={staticFile('audio/dialogue_blip.mp3')} volume={0.5} />
      </Sequence>
      <Sequence from={195} durationInFrames={45} name="SFX_Hub_Airlock">
        <Audio src={staticFile('audio/door_open.wav')} volume={0.42} />
      </Sequence>

      {/* Escena 3: Selección de los 4 componentes en el Taller y salida del Rover */}
      <Sequence from={278} durationInFrames={18} name="SFX_Workshop_Step1">
        <Audio src={staticFile('audio/click.wav')} volume={0.5} />
      </Sequence>
      <Sequence from={314} durationInFrames={18} name="SFX_Workshop_Step2">
        <Audio src={staticFile('audio/click.wav')} volume={0.5} />
      </Sequence>
      <Sequence from={350} durationInFrames={18} name="SFX_Workshop_Step3">
        <Audio src={staticFile('audio/click.wav')} volume={0.5} />
      </Sequence>
      <Sequence from={386} durationInFrames={18} name="SFX_Workshop_Step4">
        <Audio src={staticFile('audio/click.wav')} volume={0.55} />
      </Sequence>
      <Sequence from={405} durationInFrames={35} name="SFX_Workshop_Verified">
        <Audio src={staticFile('audio/star_ding.wav')} volume={0.55} />
      </Sequence>
      <Sequence from={405} durationInFrames={45} name="SFX_Workshop_Rollout">
        <Audio src={staticFile('audio/rover_motor.wav')} volume={0.38} />
      </Sequence>

      {/* Escena 4: Cuenta regresiva, ignición, despegue y enlace del Satélite Terra */}
      <Sequence from={450} durationInFrames={12} name="SFX_Countdown_3">
        <Audio src={staticFile('audio/countdown_beep.wav')} volume={0.55} />
      </Sequence>
      <Sequence from={462} durationInFrames={12} name="SFX_Countdown_2">
        <Audio src={staticFile('audio/countdown_beep.wav')} volume={0.55} />
      </Sequence>
      <Sequence from={474} durationInFrames={12} name="SFX_Countdown_1">
        <Audio src={staticFile('audio/countdown_beep.wav')} volume={0.65} />
      </Sequence>
      <Sequence from={455} durationInFrames={120} name="SFX_Rocket_Roar">
        <Audio src={staticFile('audio/rocket_roar.wav')} volume={0.62} />
      </Sequence>
      <Sequence from={578} durationInFrames={35} name="SFX_Terra_Telemetry">
        <Audio src={staticFile('audio/Generated/comms_back.wav')} volume={0.55} />
      </Sequence>

      {/* Escena 5: Exploración y escaneo en Marte */}
      <Sequence from={725} durationInFrames={45} name="SFX_Mars_Scan">
        <Audio src={staticFile('audio/scan.wav')} volume={0.58} />
      </Sequence>
      <Sequence from={765} durationInFrames={35} name="SFX_Mars_Complete">
        <Audio src={staticFile('audio/site_complete.wav')} volume={0.62} />
      </Sequence>

      {/* Escena 6: Descubrimiento de hielo en la Luna (-240 C) */}
      <Sequence from={895} durationInFrames={45} name="SFX_Moon_Ice_Scan">
        <Audio src={staticFile('audio/scan.wav')} volume={0.58} />
      </Sequence>
      <Sequence from={945} durationInFrames={35} name="SFX_Moon_Complete">
        <Audio src={staticFile('audio/site_complete.wav')} volume={0.65} />
      </Sequence>

      {/* Escena 7: Alerta de Modo Seguro -> Reconexión y misión 3/3 */}
      <Sequence from={992} durationInFrames={45} name="SFX_Safe_Mode_Alert">
        <Audio src={staticFile('audio/Generated/safe_mode.wav')} volume={0.62} />
      </Sequence>
      <Sequence from={1082} durationInFrames={35} name="SFX_Comms_Restored">
        <Audio src={staticFile('audio/Generated/comms_back.wav')} volume={0.65} />
      </Sequence>
      <Sequence from={1155} durationInFrames={40} name="SFX_Rescue_Success">
        <Audio src={staticFile('audio/site_complete.wav')} volume={0.65} />
      </Sequence>

      {/* Escena 8: 3 estrellas doradas en el Gran Final */}
      <Sequence from={1206} durationInFrames={25} name="SFX_Finale_Star1">
        <Audio src={staticFile('audio/star_ding.wav')} volume={0.5} />
      </Sequence>
      <Sequence from={1213} durationInFrames={25} name="SFX_Finale_Star2">
        <Audio src={staticFile('audio/star_ding.wav')} volume={0.55} />
      </Sequence>
      <Sequence from={1220} durationInFrames={35} name="SFX_Finale_Star3">
        <Audio src={staticFile('audio/star_ding.wav')} volume={0.65} />
      </Sequence>

      {/* =================================================================== */}
      {/* SECUENCIAS VISUALES (1350 FOTOGRAMAS EXACTOS = 45.00 SEGUNDOS)      */}
      {/* =================================================================== */}
      <Sequence
        from={0}
        durationInFrames={120}
        name="01_El_Universo_Nos_Espera (00-04s)"
      >
        <Scene01_UniverseAwaits />
      </Sequence>

      <Sequence
        from={120}
        durationInFrames={150}
        name="02_El_Comienzo_De_La_Mision (04-09s)"
      >
        <Scene02_MissionBegins />
      </Sequence>

      <Sequence
        from={270}
        durationInFrames={180}
        name="03_Disena_Tu_Mision (09-15s)"
      >
        <Scene03_DesignYourMission />
      </Sequence>

      <Sequence
        from={450}
        durationInFrames={210}
        name="04_El_Lanzamiento (15-22s)"
      >
        <Scene04_TheLaunch />
      </Sequence>

      <Sequence
        from={660}
        durationInFrames={180}
        name="05_Exploracion_De_Marte (22-28s)"
      >
        <Scene05_ExploreMars />
      </Sequence>

      <Sequence
        from={840}
        durationInFrames={150}
        name="06_El_Secreto_De_La_Luna (28-33s)"
      >
        <Scene06_MoonSecret />
      </Sequence>

      <Sequence
        from={990}
        durationInFrames={210}
        name="07_Rescate_Y_Resolucion (33-40s)"
      >
        <Scene07_ControlRescue />
      </Sequence>

      <Sequence
        from={1200}
        durationInFrames={150}
        name="08_Cierre_Cinematografico (40-45s)"
      >
        <Scene08_GrandFinale />
      </Sequence>
    </AbsoluteFill>
  );
};
