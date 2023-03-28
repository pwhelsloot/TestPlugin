import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { isTruthy } from '../is-truthy.function';

export class FileHelper {
    static getContentType(extension: string): string {
        switch (extension) {
            case '.csv':
                return 'text/csv';

            case '.xls':
                return 'application/excel';

            case '.xlsx':
                return 'application/excel';

            case '.doc':
                return 'application/msword';

            case '.docx':
                return 'application/msword';

            case '.ppt':
                return 'application/mspowerpoint';

            case '.pdf':
                return 'application/pdf';

            case '.jpg':
                return 'image/jpeg';

            case '.jpeg':
                return 'image/jpeg';

            case '.png':
                return 'image/png';

            case '.zip':
                return 'application/x-compressed';

            case '.xml':
                return 'text/xml';

            case '.rtf':
                return 'application/rtf';

            case '.html':
                return 'text/html';

            case '.txt':
                return 'text/plain';

            default:
                return 'application/unknown';
        }
    }

    static getFileNameComponents(fileName: string): { fileNameWithoutExt: string; ext: string } {
        let fileNameWithoutExt = '';
        let ext = '';

        if (isTruthy(fileName)) {
            const index = fileName.lastIndexOf('.');
            if (index !== -1) {
                fileNameWithoutExt = fileName.substr(0, index);
                ext = fileName.substr(index);
            } else {
                fileNameWithoutExt = fileName;
            }
        }
        return { fileNameWithoutExt, ext };
    }

    static convertBase64ToFile(base64: string, fileName: string): File {
        // Get file type
        const components = this.getFileNameComponents(fileName);
        const contentType = this.getContentType(components.ext);

        // Convert to blob, then File
        const blob: any = this.convertBase64ToBlob(base64, contentType);
        return this.convertBlobToFile(blob, fileName);
    }

    static convertBase64ToBlob(base64: string, contentType: string): Blob {
        const sliceSize = 512;
        const byteCharacters = atob(base64);
        const byteArrays = [];

        for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            const slice = byteCharacters.slice(offset, offset + sliceSize);

            const byteNumbers = new Array(slice.length);
            for (let i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            const byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }

        const blob = new Blob(byteArrays, { type: contentType });
        return blob;
    }

    static convertBlobToFile(blob: Blob, fileName: string): File {
        const b: any = blob;
        b.lastModifiedDate = AmcsDate.now();
        b.name = fileName;
        return blob as File;
    }

    static convertBase64ToArrayBuffer(base64: string): ArrayBuffer {
        const binaryString = window.atob(base64);
        const length = binaryString.length;
        const bytes = new Uint8Array(length);
        for (let i = 0; i < length; i++) {
            bytes[i] = binaryString.charCodeAt(i);
        }
        return bytes.buffer as ArrayBuffer;
    }
}
