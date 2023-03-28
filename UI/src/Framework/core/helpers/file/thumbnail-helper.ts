import { Injectable } from '@angular/core';
import { isTruthy } from '../is-truthy.function';
import { FileHelper } from './file-helper';

@Injectable()
export class ThumbnailHelper {
    createThumbnail(base64: string, fileName: string, maxWidth: number, maxHeight: number): Promise<string> {
        return new Promise((resolve) => {
            if (this.isFileImage(fileName)) {
                // Convert to File
                const file = FileHelper.convertBase64ToFile(base64, fileName);

                // Resize image for thumbnail
                this.resizeImage(file, maxWidth, maxHeight).then((blob) => {
                    // Convert result to string
                    const reader = new FileReader();
                    reader.readAsDataURL(blob);
                    reader.onloadend = function() {
                        resolve(reader.result as string);
                    };
                });
            } else {
                resolve(null);
            }
        });
    }

    private isFileImage(fileName: string): boolean {
        if (!isTruthy(fileName)) {
            return false;
        } else {
            const components = FileHelper.getFileNameComponents(fileName);
            const contentType = FileHelper.getContentType(components.ext);
            return contentType.startsWith('image/');
        }
    }

    private resizeImage(file: File, maxWidth: number, maxHeight: number): Promise<Blob> {
        return new Promise((resolve, reject) => {
            const image = new Image();
            image.src = URL.createObjectURL(file);
            image.onload = () => {
                const width = image.width;
                const height = image.height;

                if (width <= maxWidth && height <= maxHeight) {
                    resolve(file);
                }

                let newWidth;
                let newHeight;

                if (width > height) {
                    newHeight = height * (maxWidth / width);
                    newWidth = maxWidth;
                } else {
                    newWidth = width * (maxHeight / height);
                    newHeight = maxHeight;
                }

                const canvas = document.createElement('canvas');
                canvas.width = newWidth;
                canvas.height = newHeight;

                const context = canvas.getContext('2d');
                context.drawImage(image, 0, 0, newWidth, newHeight);
                canvas.toBlob(resolve, file.type);
            };
            image.onerror = reject;
        });
    }
}
