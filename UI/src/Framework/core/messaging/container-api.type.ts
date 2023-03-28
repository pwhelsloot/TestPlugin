import { IPlatformToPluginApi } from '@amcs/platform-communication';
import { AsyncMethodReturns } from 'penpal';

export type ContainerAppApi = AsyncMethodReturns<IPlatformToPluginApi> | IPlatformToPluginApi;
