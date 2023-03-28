import { alias } from '@coreconfig/api-dto-alias.function';

export class BytePicture {

    @alias('PictureBase64')
    pictureBase64: string;

    @alias('Caption')
    caption: string;
}
