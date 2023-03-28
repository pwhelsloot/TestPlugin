
export function buildClass(destination: any, sourceArgs: any, mappingFields: any): any {

    for (const field in mappingFields) {
        if (mappingFields[field]) {
            const aliasField = mappingFields[field];
            destination[field] = sourceArgs[aliasField];
        }
    }

    return destination;
}
