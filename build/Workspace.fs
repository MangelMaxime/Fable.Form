module EasyBuild.Workspace

open EasyBuild.FileSystemProvider

[<Literal>]
let root = __SOURCE_DIRECTORY__ + "/../"

type Workspace = RelativeFileSystem<root>

type VirtualWorkspace =
    VirtualFileSystem<
        root,
        """
examples
    fableBuild
tests
    fableBuild
"""
     >
