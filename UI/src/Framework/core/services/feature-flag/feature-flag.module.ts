import { NgModule } from '@angular/core';
import { ConfigurationStorageService } from './configuration-storage';
import { ConfigurationUpdateService } from './configuration-update.service';
import { ConfigurationService } from './configuration.service';

@NgModule({
  providers: [ConfigurationUpdateService, ConfigurationStorageService, ConfigurationService]
})
export class FeatureFlagModule {}
