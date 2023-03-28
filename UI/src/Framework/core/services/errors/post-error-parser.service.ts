export class PostErrorParserService {

    static ErrorStart = '/***';
    static ErrorEnd = '***/';
    static NewLine = '***/n***';

    static parseError(errorMessage: string): string {
        const requiresParsing = errorMessage.startsWith(this.ErrorStart) &&
        errorMessage.endsWith(this.ErrorEnd);
        if (requiresParsing) {
            errorMessage = errorMessage.substring(4, errorMessage.length - 4);
            errorMessage = errorMessage.replace(new RegExp(this.escapeRegExp(this.NewLine), 'g'), '\n');
        }
        return errorMessage;
    }

    private static escapeRegExp(search: string) {
        return search.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
      }
}
