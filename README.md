# Temat
Aplikacja symulująca sklep internetowy (e-commerce)
# Opis
Za pomocą aplikacji można wyświetlić listę produktów w sklepie.
Użytkownik ma możliwość dodania, anulowania oraz wyświetlenia zamówienia.
# Schemat BD
```sql
CREATE TABLE Products (
  productname varchar,
  category varchar,
  amount int,
  price double,
  PRIMARY KEY (category, productname)
);
```

```sql
CREATE TABLE Orders (
  orderid uuid, 
  clientname varchar,
  date timestamp,
  productname varchar,
  category varchar,
  amount int,
  deliveryaddress varchar,
  status varchar,
  price double,
  PRIMARY KEY (clientname, orderid, productname)
);
```
# Funkcjonalności
- Wyświetlanie produktów (nazwa, cena, kategoria, ilość w magazynie),
- Zmiana ilości dostępnych produktów (dla producenta),
- Stwórz produkt (dla producenta),
- Zrealizuj zamówienie,
- Wyświetlanie zamówienia,
- Sprawdź zamówienie,
- Usuń zamówienie,
- Dodaj produkt do koszyka,
- Usuń produkt z koszyka,
- Przejrzyj koszyk,
- Zamów zawartość koszyka,
- Wygeneruj przeciążenie,
- Własna komenda.

# Problem
Zbyt duża ilość jednoczesnych zamówień prowadząca do potwierdzenia kupna w przypadku braku towaru (niezaktualizowane ilości na węzłach).
# Podsumowanie
Rozwiązanie zaproponowane przez nas zostało napisane w języku C# z wykorzystaniem DataStax Driver dla C#. Charakteryzuje się wysoką dokładnością (0,01%-0,02%).

Wygenerowane obciążenia polega na równoległym wysyłaniu zapytań przez wątki, które zwiększają ilość produktów na stanie magazynu.


