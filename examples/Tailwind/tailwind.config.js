import daisyui from 'daisyui';
import themes from 'daisyui/src/theming/themes';
import { scopedPreflightStyles, isolateInsideOfContainer } from 'tailwindcss-scoped-preflight';

/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./../../docs_deploy/**/*.html",
        "./../../docs_deploy/**/*.js",
        "./../Sutil/src/Fields/DaisyInput.fs",
        "./../Sutil/src/Pages/CustomView.fs",
    ],
    theme: {
        extend: {},
    },
    plugins: [
        scopedPreflightStyles({
            isolationStrategy: isolateInsideOfContainer(".daisyui"),
        }),
        daisyui,
    ],
    daisyui: {
        prefix: 'daisy-',
        themes: [
            {
                ligth: {
                    ...themes.light,
                    'primary': '#176663',
                }
            }
        ]
    },
    prefix: "tw-"
}
