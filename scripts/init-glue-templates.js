const initialChangelog = () => {
    return `
    # Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased
`.trimStart()
}

const initialReadme = (glueName, npmPackageName, npmPackageUrl) => {
    return `
# Glutinum.${glueName}

Binding for [${npmPackageName}](${npmPackageUrl})

## Usage
`.trimStart()
}

const initialGlueFsproj = (glueName, authors, npmPackageUrl) => {
    return `
<?xml version="1.0" encoding="utf-8"?>
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <Version>0.0.0</Version>
        <TargetFramework>netstandard2.0</TargetFramework>
        <GenerateDocumentationFile>true</GenerateDocumentationFile>
        <Authors>${authors}</Authors>
        <Description>
            Fable bindings for npm ${npmPackageUrl} package
        </Description>
    </PropertyGroup>
    <ItemGroup>
        <Compile Include="Glutinum.${glueName}.fs" />
    </ItemGroup>
    <!-- This package doesn't contain actual code
        so we don't need to add the sources -->
    <!-- <ItemGroup>
    <Content Include="*.fsproj; *.fs" PackagePath="fable\" />
    </ItemGroup> -->
</Project>
`.trimStart()
}

const lowerFirstLetter = (txt) => {
    return txt.charAt(0).toLowerCase() + txt.slice(1)
}

const initialGlueFsharpFile = (glueName, npmPackageName) => {
    return `
module rec Glutinum.${glueName}

open Fable.Core

[<Import("default", "${npmPackageName}")>]
let ${lowerFirstLetter(glueName)} : ${glueName}.IExports = jsNative

module ${glueName} =

    type [<AllowNullLiteral>] IExports =
        class end
`.trimStart()
}

const initialGlueTestFsproj = (glueName) => {
    return `
<?xml version="1.0" encoding="utf-8"?>
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
        <GenerateDocumentationFile>true</GenerateDocumentationFile>
        <RelativePathToTestsShared>../../../tests-shared</RelativePathToTestsShared>
    </PropertyGroup>
    <Import Project="$(RelativePathToTestsShared)/Tests.Shared.props" />
    <ItemGroup>
        <Compile Include="Tests.${glueName}.fs" />
    </ItemGroup>
    <ItemGroup>
        <ProjectReference Include="../src/Glutinum.${glueName}.fsproj" />
    </ItemGroup>
</Project>
`.trimStart()
}

const initialGlueTestFsharpFile = (glueName) => {
    return `
module Tests.${glueName}

open Mocha
open Fable.Core
open Glutinum.${glueName}

describe "${glueName}" (fun _ ->

    it "Mime new constructor works" (fun _ ->
        failwith "Tests should go here"
    )
)
`.trimStart()
}

exports.initialChangelog = initialChangelog
exports.initialReadme = initialReadme
exports.initialGlueFsproj = initialGlueFsproj
exports.initialGlueFsharpFile = initialGlueFsharpFile
exports.initialGlueTestFsproj = initialGlueTestFsproj
exports.initialGlueTestFsharpFile = initialGlueTestFsharpFile
