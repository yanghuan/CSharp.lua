require("run")

run {
 depth = 2,
 input = "src",
 output = "out",
 libs = "Bridge/Bridge.dll",
 metaFiles = "Bridge/Bridge.xml",
 attr = "TestCase",
 metadata = true,
}