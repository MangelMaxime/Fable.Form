const standard = require('nacara/dist/layouts/standard/Export').default;
const mdMessage = require('nacara/dist/js/utils').mdMessage;

module.exports = {
    githubURL: "https://github.com/MangelMaxime/Fable.Form",
    url: "https://mangelmaxime.github.io",
    baseUrl: "/Fable.Form/",
    editUrl : "https://github.com/MangelMaxime/Fable.Form/edit/main/docs",
    source: "docs",
    serverPort: 8081,
    output: "temp",
    title: "Fable.Form",
    changelog: "CHANGELOG.md",
    version: "0.0.0",
    navbar: {
        showVersion: false,
        links: [
            {
                href: "/Fable.Form/index.html",
                label: "Documentation",
                icon: "fas fa-book"
            },
            {
                href: "/Fable.Form/examples/index.html",
                label: "Examples",
                icon: "fas fa-list-ul fa-lg"
            },
            {
                href: "https://gitter.im/fable-compiler/Fable",
                label: "Support",
                icon: "fab fa-gitter",
                isExternal: true
            },
            {
                href: "https://github.com/MangelMaxime/Fable.Form",
                icon: "fab fa-github",
                isExternal: true
            },
            {
                href: "https://twitter.com/MangelMaxime",
                icon: "fab fa-twitter",
                isExternal: true,
                color: "#55acee"
            }
        ]
    },
    menu: {
        "Getting Started": [
            "index"
        ]
    },
    layouts: {
        default: standard.Default,
        changelog: standard.Changelog
    },
    plugins: {
        markdown: [
            {
                path: 'markdown-it-container',
                args: [
                    'warning',
                    mdMessage("warning")
                ]
            },
            {
                path: 'markdown-it-container',
                args: [
                    'info',
                    mdMessage("info")
                ]
            },
            {
                path: 'markdown-it-container',
                args: [
                    'success',
                    mdMessage("success")
                ]
            },
            {
                path: 'markdown-it-container',
                args: [
                    'danger',
                    mdMessage("danger")
                ]
            },
            {
                path: 'nacara/dist/js/markdown-it-anchored.js'
            },
            {
                path: 'nacara/dist/js/markdown-it-toc.js'
            }
        ]
    }
};
