export interface ILookupItem {
    id: number;
    description: string;
    guid?: string; // All api items send this it's just not on all models
}
