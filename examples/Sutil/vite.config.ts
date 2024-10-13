import { defineConfig } from "vite";


export default defineConfig(({ mode}) => {

    const isProduction = mode === "production";

    return {
        clearScreen: false,
        build: {
            rollupOptions: {
                output: {
                    entryFileNames: "[name].js",
                    dir: isProduction ? "../../docs_deploy/examples/sutil/dist/" : "dist-dev",
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
