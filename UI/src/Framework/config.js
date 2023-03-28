module.exports = {
    platform: 'azure',
    endpoint: 'https://dev.azure.com/amcsgroup/',
    token: process.env.TOKEN,
    hostRules: [
        {
            hostType: 'npm',
            matchHost: 'pkgs.dev.azure.com',
            username: 'apikey',
            password: process.env.TOKEN,
        },
        {
            hostType: 'nuget',
            matchHost: 'pkgs.dev.azure.com',
            username: 'apikey',
            password: process.env.TOKEN,
        },
        {
            matchHost: 'github.com',
            token: process.env.GITHUB_COM_TOKEN,
        },
    ],
    repositories: ['Platform/PlatformUIFramework'],
};
