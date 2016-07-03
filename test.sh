mkdir -p ./test/compiled
for in_file in $(ls -1 ./test | grep .vb)
do
	out_file="${in_file%.*}".cs
	mono ./VBTranslator/VBTranslator/bin/Debug/VBTranslator.exe ./test/$in_file ./test/compiled/$out_file > /dev/null
	mcs ./test/compiled/$out_file -reference:System.Windows.Forms > /dev/null
	exe_file="${in_file%.*}".exe
	mono ./test/compiled/$exe_file
done
# rm -f ./test/compiled/*.cs ./test/compiled/*.exe
