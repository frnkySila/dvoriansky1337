java -jar antlr-4.5.3-complete.jar VisualBasic.g4 -o VBTranslator/VBTranslator
xbuild VBTranslator/VBTranslator.sln
mkdir -p ./res
cp ./VBTranslator/VBTranslator/bin/Debug/* ./res
