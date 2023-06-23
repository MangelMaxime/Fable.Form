import { defineConfig } from "vite";


export default defineConfig(({ mode}) => {

    const isProduction = mode === "production";

    return {
        build: {
            rollupOptions: {
                output: {
                    entryFileNames: "[name].js",
                    dir: isProduction ? "../docs_deploy/examples/dist/" : "dist-dev",
                }
            }
        },
        server: {
            watch: {
                ignored: [
                    "**/*.fs", // Don't watch F# files
                ],
            },
        },
    };
});
