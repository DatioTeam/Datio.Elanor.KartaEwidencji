## Opis

Dodatek umożliwia seryjne generowanie wydruku **EwidencjaCzasuPracySzczegolowa**. Drukowany jest 1 wydruk dla 1 pracownika i całość zapisywana jest do ZIPa.

## Gdzie?

Przycisk do generowania raportów dostępny jest na liście pracowników, zarówno w Menu jak i na samej belce pod nazwą **Generuj kartę ewidencji**

## Założenia 

Zgłoszenie: SER/2024/11/57

Funkcjonalność ma za zadanie automatycznie wygenerować wydruki do PDF oraz spakować je do archiwum zip i zapisać we wskazanej lokalizacji na dysku. Każdy plik pdf będzie miał określony schemat nazewnictwa i będzie zawierał informacje tylko dla jednego pracownika:

Nazwa pliku docelowego dla jednego pracownika:

`{miesiąc}_{rok}_{nazwafirmy}_{imię}_{nazwisko}_{kod}.pdf`

Gdzie:
  - **miesiąc** – miesiąc z parametru raportu
  - **rok** – rok z parametru raportu,
  - **nazwafirmy** – oznaczenie firmy, cecha globalna,
  - **imię i nazwisko** – informacje z pracownika aktualne na ostatni dzień okresu (z uwagi na możliwość zmiany nazwiska w trakcie trwania okresu wskazanego w raporcie).
  - **kod** – kod pracownika, zapobiegający potencjalnej duplikacji plików w przypadku kiedy w systemie znajdują się osoby o tym samym imieniu i nazwisku,

Plik archiwum zip będzie zawierał wszystkie wygenerowane pliki pdf.

