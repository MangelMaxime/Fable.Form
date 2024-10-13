module EasyBuild.Workspace

open EasyBuild.FileSystemProvider

[<Literal>]
let root = __SOURCE_DIRECTORY__ + "/../"

type Workspace = RelativeFileSystem<root>

type VirtualWorkspace =
    VirtualFileSystem<
        root,
        """
examples/
    React/
        fableBuild/
        public/
            daisyui.css
    Sutil/
        fableBuild/
        public/
            daisyui.css
    Lit/
        fableBuild/
        public/
            daisyui.css
tests/
    fableBuild/
docs_deploy/
    examples/
        daisyui.css
"""
     >
