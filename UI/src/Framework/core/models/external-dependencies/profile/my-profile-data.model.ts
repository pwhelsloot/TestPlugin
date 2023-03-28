import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated Move to ALL UI apps (which actually use MyProfile)
 */
export class MyProfileData {
  @alias('UiLanguage')
  uiLanguage: string;

  @alias('Forename')
  forename: string;

  @alias('Surname')
  surname: string;

  @alias('SysUserId')
  sysUserId: number;

  @alias('StaffType')
  staffType: string;

  @alias('UserInitials')
  userInitials: string;

  @alias('SysUserPictureId')
  sysUserPictureId: number;

  @alias('PictureId')
  pictureId: number;

  @alias('ProfilePictureBase64')
  profilePictureBase64: string;

  @alias('ThumbnailBase64')
  thumbnailBase64: string;

  @alias('PrinterId')
  printerId: number;

  @alias('AutoPrint')
  autoPrint: boolean;
}
