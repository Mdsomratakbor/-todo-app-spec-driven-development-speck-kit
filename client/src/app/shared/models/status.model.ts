export interface Status {
  id: number;
  name: string;
  color: string;
}

export const STATUS_OPTIONS: Status[] = [
  { id: 1, name: 'Pending', color: '#95A5A6' },
  { id: 2, name: 'In Progress', color: '#3498DB' },
  { id: 3, name: 'Completed', color: '#2ECC71' },
];

export const STATUS_TRANSITIONS: Record<number, number[]> = {
  1: [2],
  2: [3],
  3: [],
};
