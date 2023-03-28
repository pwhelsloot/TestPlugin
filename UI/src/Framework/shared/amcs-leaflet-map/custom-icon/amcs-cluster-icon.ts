
import { DivIcon, MarkerCluster, Point } from 'leaflet';

export class AmcsClusterIcon extends DivIcon {
}

export function amcsClusterIcon(cluster: MarkerCluster, customColor: string): AmcsClusterIcon {
    const childCount = cluster.getChildCount();
    let clusterIcon = new DivIcon({
        html: '<div style=\"border: 5px solid ' + customColor + '\"><span>' + childCount + '</span></div>',
        className: 'marker-cluster',
        iconSize: new Point(40, 40)
    });
    return clusterIcon;
}
