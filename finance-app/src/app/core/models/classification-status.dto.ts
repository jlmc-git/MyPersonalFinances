export type ClassificationStatusDto =
  | {
      status: 'Pending';
    }
  | {
      status: 'Classified';
      categoryId: string;
      confidence: number;
    }
  | {
      status: 'Rejected';
      reason: string;
    };
