/**
 * @deprecated To be deleted
 */
export class CountryCoordinates {

    static getCoord(country: string) {
        return this.coords.find(x => x.country === country);
    }

    private static coords = [
        { country: 'United States of America', lat: 37.09024, long: -95.712891 },
        { country: 'United Kingdom', lat: 55.378051, long: -3.435973 },
        { country: 'Sweden', lat: 60.128161, long: 18.643501 },
        { country: 'Norway', lat: 60.472024, long: 8.468946 },
        { country: 'France', lat: 46.227638, long: 2.213749 },
        { country: 'Ireland', lat: 53.41291, long: -8.24389 },
    ];
}
