# Installs and runs vstest.console for running tests on MAC OS X CI build.
# By: Christian Gunderman

# Install vstest.console.
nuget install "Microsoft.TestPlatform" -version "16.2" || exit 1

# Run all tests.
mono ./Microsoft.TestPlatform.16.2.0/tools/net451/Common7/IDE/Extensions/TestPlatform/vstest.console.exe ../bin/**/Release/net472/*Tests.dll || exit 2
