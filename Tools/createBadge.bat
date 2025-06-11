pushd %~dp0
ucd.exe auth login a1b517d9836bf74a536c0b3971c2f2be
ucd.exe config set bucket %2
ucd.exe badges add %1 %3 -b %2
popd