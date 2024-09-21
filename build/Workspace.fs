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
    react/
        fableBuild/
    sutil/
        fableBuild/
tests/
    fableBuild/
"""
     >
