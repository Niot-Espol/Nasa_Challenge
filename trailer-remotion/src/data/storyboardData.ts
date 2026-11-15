export interface SceneSpec {
  id: string;
  number: number;
  title: string;
  keyword?: string;
  startFrame: number;
  durationInFrames: number;
  startSec: number;
  endSec: number;
  narrativeLine: string;
}

export const TRAILER_SCENES: SceneSpec[] = [
  {
    id: 'Scene01_UniverseAwaits',
    number: 1,
    title: 'EL UNIVERSO NOS ESPERA',
    startFrame: 0,
    durationInFrames: 120, // 0–4s (4s)
    startSec: 0,
    endSec: 4,
    narrativeLine: 'Cada gran descubrimiento comienza con una decisión.',
  },
  {
    id: 'Scene02_MissionBegins',
    number: 2,
    title: 'EL COMIENZO DE LA MISIÓN',
    startFrame: 120,
    durationInFrames: 150, // 4–9s (5s)
    startSec: 4,
    endSec: 9,
    narrativeLine: 'Dra. Elena Vega y BIMO te dan el mando del Programa Mars Crafts.',
  },
  {
    id: 'Scene03_DesignYourMission',
    number: 3,
    title: 'DISEÑA TU MISIÓN',
    keyword: 'DISEÑA',
    startFrame: 270,
    durationInFrames: 180, // 9–15s (6s)
    startSec: 9,
    endSec: 15,
    narrativeLine: 'Energía, masa, instrumentos y presupuesto: ninguna configuración es trivial.',
  },
  {
    id: 'Scene04_TheLaunch',
    number: 4,
    title: 'EL LANZAMIENTO',
    startFrame: 450,
    durationInFrames: 210, // 15–22s (7s)
    startSec: 15,
    endSec: 22,
    narrativeLine: 'De la plataforma de despegue al espacio interplanetario.',
  },
  {
    id: 'Scene05_ExploreMars',
    number: 5,
    title: 'EXPLORACIÓN DE MARTE',
    keyword: 'EXPLORA',
    startFrame: 660,
    durationInFrames: 180, // 22–28s (6s)
    startSec: 22,
    endSec: 28,
    narrativeLine: 'Cráter Jezero: antiguos deltas, tormentas de polvo y ciencia real de NASA.',
  },
  {
    id: 'Scene06_MoonSecret',
    number: 6,
    title: 'EL SECRETO DE LA LUNA',
    keyword: 'DESCUBRE',
    startFrame: 840,
    durationInFrames: 150, // 28–33s (5s)
    startSec: 28,
    endSec: 33,
    narrativeLine: 'Polo sur lunar: frío extremo y hielo de agua en sombra permanente.',
  },
  {
    id: 'Scene07_ControlRescue',
    number: 7,
    title: 'DIAGNÓSTICO Y RESCATE',
    keyword: 'RESUELVE',
    startFrame: 990,
    durationInFrames: 210, // 33–40s (7s)
    startSec: 33,
    endSec: 40,
    narrativeLine: 'Analiza la telemetría, ajusta la antena y los sistemas, y salva la misión.',
  },
  {
    id: 'Scene08_GrandFinale',
    number: 8,
    title: 'CIERRE CINEMATOGRÁFICO',
    startFrame: 1200,
    durationInFrames: 150, // 40–45s (5s)
    startSec: 40,
    endSec: 45,
    narrativeLine: 'MARS CRAFTS — Diseña. Explora. Descubre.',
  },
];

export interface GameplayCaptureSpec {
  id: string;
  fileName: string;
  unityScene: string;
  priority: 'P1' | 'P2';
  usedInScene: string;
  trailerWindowSec: string;
  recommendedCaptureDurationSec: number;
  playerAction: string;
  framing: string;
}

export const GAMEPLAY_CAPTURE_LIST: GameplayCaptureSpec[] = [
  {
    id: 'clip01',
    fileName: '01_hub_briefing.mp4',
    unityScene: '01_Hub.unity',
    priority: 'P1',
    usedInScene: 'Escena 2 (06.8s – 08.8s)',
    trailerWindowSec: '2.0 s',
    recommendedCaptureDurationSec: 6,
    playerAction:
      'Camina con el astronauta desde el centro del Hub hacia la Dra. Elena Vega y BIMO frente a la pantalla principal y abre el diálogo de briefing.',
    framing: '1920×1080 pantalla completa en Play Mode (cámara centrada en el Hub).',
  },
  {
    id: 'clip02',
    fileName: '02_workshop_design.mp4',
    unityScene: '02_Workshop.unity',
    priority: 'P1',
    usedInScene: 'Escena 3 (09.5s – 14.5s, 3 cortes rápidos)',
    trailerWindowSec: '5.0 s (3 cortes de ~1.5 s)',
    recommendedCaptureDurationSec: 12,
    playerAction:
      'Alterna entre Paneles Solares y RTG, Ruedas Estándar y Todoterreno, Antena Directa y Relé Orbital, y añade Cámara + Espectrómetro + Taladro viendo cómo suben las barras de Presupuesto y Masa y aparece el aviso de BIMO antes de quedar válido el botón Lanzar.',
    framing: '1920×1080 mostrando las tarjetas de componentes, las barras superiores y el panel derecho de BIMO.',
  },
  {
    id: 'clip03',
    fileName: '04_mars_exploration.mp4',
    unityScene: '04_Mars.unity',
    priority: 'P1',
    usedInScene: 'Escena 5 (22.5s – 27.8s, 3 cortes rápidos)',
    trailerWindowSec: '5.3 s',
    recommendedCaptureDurationSec: 15,
    playerAction:
      'Conduce el rover hacia el punto científico del Delta, pulsa E para escanear (que se vea la animación de escaneo y el popup con el dato NASA de Perseverance) y captura también unos segundos durante la tormenta de polvo (t=45 s).',
    framing: '1920×1080 con el HUD superior visible (Energía, Sols, Ciencia, Estado).',
  },
  {
    id: 'clip04',
    fileName: '04_moon_ice_crater.mp4',
    unityScene: '04_Moon.unity',
    priority: 'P1',
    usedInScene: 'Escena 6 (28.5s – 32.8s, 2 cortes rápidos)',
    trailerWindowSec: '4.3 s',
    recommendedCaptureDurationSec: 12,
    playerAction:
      'Conduce desde el borde iluminado hacia el interior del cráter en sombra permanente (abajo a la izquierda), donde bajan la temperatura/energía, y escanea el depósito de hielo de agua azul brillante.',
    framing: '1920×1080 mostrando la entrada al anillo del cráter oscuro y el escaneo del hielo.',
  },
  {
    id: 'clip05',
    fileName: '05_report_and_rescue.mp4',
    unityScene: '05_Report.unity + 02_Workshop.unity',
    priority: 'P2',
    usedInScene: 'Escena 7 (35.2s – 39.2s, 2 cortes rápidos)',
    trailerWindowSec: '4.0 s',
    recommendedCaptureDurationSec: 10,
    playerAction:
      'Muestra el Reporte de misión explicando la causa de un incidente (modo seguro o pérdida de señal) y pulsa el botón "Rediseñar" para volver al Taller y equipar el Relé Vía Orbitador y las Ruedas Todoterreno.',
    framing: '1920×1080 enfocando las líneas de diagnóstico causal y la comparación con NASA.',
  },
];
