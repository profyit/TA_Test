export interface Task {
  id?: string | number;
  title: string;
  description: string;
  dueDate: Date | string;
  isCompleted: boolean;
}
