export interface Priority {
  id: number;
  name: string;
  color: string;
}

export const PRIORITY_OPTIONS: Priority[] = [
  { id: 1, name: 'Low', color: '#2ECC71' },
  { id: 2, name: 'Medium', color: '#3498DB' },
  { id: 3, name: 'High', color: '#F39C12' },
  { id: 4, name: 'Critical', color: '#E74C3C' },
];
