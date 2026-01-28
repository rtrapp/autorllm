export interface Chapter {
  id: string;
  projectId: string;
  title: string;
  summary: string;
  content: string;
  order: number;
  wordCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateChapterInput {
  title: string;
  summary?: string;
}

export interface UpdateChapterInput {
  chapterId: string;
  title?: string;
  summary?: string;
  content?: string;
}
