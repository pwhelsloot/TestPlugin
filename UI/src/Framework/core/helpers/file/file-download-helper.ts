import { Injectable } from '@angular/core';
import * as XLSX from 'xlsx';
import { IExcelItem } from '../iexcel-item.interface';
import { isTruthy } from '../is-truthy.function';
import { FileHelper } from './file-helper';

function print(): (this: WindowEventHandlers, ev: Event) => any {
    const frame = document.getElementById('iframe-print') as HTMLIFrameElement;
    if (frame != null) {
        frame.contentWindow.onafterprint = closePrint;
        frame.contentWindow.onbeforeunload = closePrint;
        frame.contentWindow.focus();
        try {
            frame.contentWindow.document.execCommand('print', false, null);
        } catch (error) {
            frame.contentWindow.print();
        }
    }
    return null;
}

function closePrint(): (this: WindowEventHandlers, ev: Event) => any {
    document.body.removeChild(document.getElementById('iframe-print'));
    return null;
}

@Injectable()
export class FileDownloadHelper {
    downloadFile(name: string, extension: string, content: string) {
        const byteArray = FileHelper.convertBase64ToArrayBuffer(content);
        const blob = new Blob([byteArray], { type: FileHelper.getContentType(extension) });

        const a = document.createElement('a');
        document.body.appendChild(a);
        const objectUrl = URL.createObjectURL(blob);
        a.href = objectUrl;

        // Append the extension to the name if it does not already end with the extension
        name = name.trim();
        if (isTruthy(extension)) {
            extension = extension.trim();
            if (!name.endsWith(extension)) {
                name += extension;
            }
        }

        a.download = name;
        a.click();
        URL.revokeObjectURL(objectUrl);
    }

    downloadBlob(fileName: string, response: Blob) {
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(response);
        link.download = fileName;
        link.target = '_blank';
        link.click();
        return null;
    }

    printBlob(response: Blob) {
        const frame = document.createElement('iframe');
        frame.onload = print;
        frame.id = 'iframe-print';
        frame.style.position = 'fixed';
        frame.style.right = '0';
        frame.style.bottom = '0';
        frame.style.width = '0';
        frame.style.height = '0';
        frame.style.border = '0';
        frame.src = window.URL.createObjectURL(response);
        document.body.appendChild(frame);
        const Iframe = document.getElementById('iframe-print') as HTMLIFrameElement;
        Iframe.contentDocument.location.reload();
        return null;
    }

    printFile(extension: string, content: string) {
        const byteArray = FileHelper.convertBase64ToArrayBuffer(content);
        const blob = new Blob([byteArray], { type: FileHelper.getContentType(extension) });

        this.printBlob(blob);
    }

    exportArrayToExcel(content: IExcelItem[], sheetName: string, fileName: string) {
        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.json_to_sheet<IExcelItem>(content);
        XLSX.utils.book_append_sheet(wb, ws, sheetName);
        XLSX.writeFile(wb, fileName);
    }
}
