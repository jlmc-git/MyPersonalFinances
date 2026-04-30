export type AppError =
  | {
      type: 'network';
      message: string;
    }
  | {
      type: 'badRequest';
      message: string;
      details?: unknown;
    }
  | {
      type: 'unauthorized';
      message: string;
    }
  | {
      type: 'forbidden';
      message: string;
    }
  | {
      type: 'notFound';
      message: string;
    }
  | {
      type: 'server';
      message: string;
      status: number;
      details?: unknown;
    }
  | {
      type: 'unknown';
      message: string;
      details?: unknown;
    };
