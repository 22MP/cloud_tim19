import type { NectarDelta, TelemetrySample } from "../../../api/telemetry";

const WIDTH = 680;
const HEIGHT = 220;
const PADDING = 34;

function scale(value: number, min: number, max: number, outputMin: number, outputMax: number) {
  if (max === min) return (outputMin + outputMax) / 2;
  return outputMin + ((value - min) / (max - min)) * (outputMax - outputMin);
}

export function LineChart({ samples }: { samples: TelemetrySample[] }) {
  const ordered = [...samples].sort((a, b) => new Date(a.recordedAt).getTime() - new Date(b.recordedAt).getTime());
  const values = ordered.flatMap((s) => [s.temperature, s.humidity]);
  const min = Math.min(...values, 0);
  const max = Math.max(...values, 1);

  const buildPoints = (selector: (sample: TelemetrySample) => number) =>
    ordered
      .map((sample, index) => {
        const x = scale(index, 0, Math.max(ordered.length - 1, 1), PADDING, WIDTH - PADDING);
        const y = scale(selector(sample), min, max, HEIGHT - PADDING, PADDING);
        return `${x},${y}`;
      })
      .join(" ");

  if (ordered.length === 0) {
    return <div className="text-sm text-gray-500 py-12 text-center">No telemetry samples for selected period.</div>;
  }

  return (
    <div className="overflow-x-auto">
      <svg viewBox={`0 0 ${WIDTH} ${HEIGHT}`} className="w-full min-w-[520px]">
        <line x1={PADDING} y1={HEIGHT - PADDING} x2={WIDTH - PADDING} y2={HEIGHT - PADDING} stroke="currentColor" className="text-gray-200" />
        <line x1={PADDING} y1={PADDING} x2={PADDING} y2={HEIGHT - PADDING} stroke="currentColor" className="text-gray-200" />
        <polyline fill="none" stroke="currentColor" strokeWidth="3" points={buildPoints((s) => s.temperature)} className="text-orange-500" />
        <polyline fill="none" stroke="currentColor" strokeWidth="3" points={buildPoints((s) => s.humidity)} className="text-blue-500" />
        {ordered.map((sample, index) => {
          const x = scale(index, 0, Math.max(ordered.length - 1, 1), PADDING, WIDTH - PADDING);
          return <text key={sample.id} x={x} y={HEIGHT - 8} fontSize="10" textAnchor="middle" className="fill-gray-400">{new Date(sample.recordedAt).getHours()}h</text>;
        })}
      </svg>
      <div className="flex gap-4 text-xs mt-2">
        <span className="text-orange-600 font-medium">● Temperature</span>
        <span className="text-blue-600 font-medium">● Humidity</span>
      </div>
    </div>
  );
}

export function BarChart({ deltas }: { deltas: NectarDelta[] }) {
  const ordered = [...deltas].sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
  const maxAbs = Math.max(...ordered.map((d) => Math.abs(d.deltaKg)), 1);
  const zeroY = HEIGHT / 2;
  const barWidth = Math.max(18, (WIDTH - PADDING * 2) / Math.max(ordered.length, 1) - 12);

  if (ordered.length === 0) {
    return <div className="text-sm text-gray-500 py-12 text-center">No nectar delta data for selected period.</div>;
  }

  return (
    <div className="overflow-x-auto">
      <svg viewBox={`0 0 ${WIDTH} ${HEIGHT}`} className="w-full min-w-[520px]">
        <line x1={PADDING} y1={zeroY} x2={WIDTH - PADDING} y2={zeroY} stroke="currentColor" className="text-gray-300" />
        {ordered.map((delta, index) => {
          const x = scale(index, 0, Math.max(ordered.length - 1, 1), PADDING + 10, WIDTH - PADDING - barWidth);
          const height = Math.abs(delta.deltaKg / maxAbs) * (HEIGHT / 2 - PADDING);
          const y = delta.deltaKg >= 0 ? zeroY - height : zeroY;
          const cls = delta.deltaKg >= 0 ? "text-green-500" : "text-red-500";
          return (
            <g key={delta.date}>
              <rect x={x} y={y} width={barWidth} height={height || 2} rx="4" fill="currentColor" className={cls} />
              <text x={x + barWidth / 2} y={delta.deltaKg >= 0 ? y - 6 : y + height + 14} fontSize="11" textAnchor="middle" className="fill-gray-600">{delta.deltaKg}</text>
              <text x={x + barWidth / 2} y={HEIGHT - 8} fontSize="10" textAnchor="middle" className="fill-gray-400">{new Date(delta.date).toLocaleDateString()}</text>
            </g>
          );
        })}
      </svg>
      <div className="flex gap-4 text-xs mt-2">
        <span className="text-green-600 font-medium">● Nectar intake</span>
        <span className="text-red-600 font-medium">● Supply consumption</span>
      </div>
    </div>
  );
}
