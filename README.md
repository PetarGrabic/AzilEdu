# Domaći zadatak — Predavanje 8 — Rezultati testiranja

## 1. Tablica rezultata (radi / ne radi)

| Cjelina | Status | Napomena |
| --- | --- | --- |
| CRUD i filtri donacija | radi | Kreiran donator, novčana i materijalna donacija, uređivanje, filter po donatoru i statusu, brisanje — sve testirano preko API-ja end-to-end. |
| Validacija donacija (API + forme) | radi | Datum u budućnosti, iznos ≤ 0, količina/vrijednost < 0, materijalna donacija bez naziva/količine — sve vraćaju `400 BadRequest` s porukom; iste poruke prikazane u `DonationsCreate`/`DonationsEdit` bez pucanja Blazor circuita (testirano uživo u pregledniku). |
| Potvrda prije brisanja donacije | radi | `IDialogService.ShowMessageBoxAsync` prikazuje dijalog; potvrđeno brisanje briše zapis i osvježava tablicu (testirano uživo). |
| Dashboard podaci | radi | `api/dashboard/summary` i `api/dashboard/recent-donations` vraćaju točne agregate; kartice na `Home.razor` prikazuju iste brojeve. |
| Upload, promjena i brisanje naslovne slike | radi | Uploadane 2 slike + 1 video; druga slika postavljena kao naslovna (vidljivo na profilu i na popisu); brisanje naslovne slike briše datoteku s diska i prebacuje naslovnu na sljedeću dostupnu sliku. |
| Odbijanje neispravnog uploada | radi | `.txt` datoteka, prazna datoteka i datoteka > 25 MB odbijene su s `400`, broj zapisa u `AnimalMedia` ostao nepromijenjen. |
| Prijava i odjava | radi | Sva 4 demo korisnika prijavljena/odjavljena u istoj Blazor sesiji; odjava briše korisnika iz `CurrentUserService`, odmah osvježava izbornik i vraća na `/login`. |
| Navigacija za sve četiri uloge | radi | Nakon ispravke (uklonjen suvišan link "Volonteri" iz Admin/Employee grupe), izbornik za sve 4 uloge točno odgovara traženoj tablici. |
| `MyTasks.razor` (ukupno / nezavršeno / upozorenje o kašnjenju) | radi | Kartice i upozorenje računaju se iz liste dohvaćene filterom `volunteerId`, bez dohvaćanja svih zapisa. |
| `MyDonations.razor` (ukupno / zbroj novčanih / zbroj procijenjene vrijednosti) | radi | Kartice se računaju iz liste dohvaćene filterom `donorId`. |
| Home.razor kartica prema ulozi | radi | Volonter vidi broj nezavršenih zadataka, donator broj svojih donacija; admin/djelatnik zadržavaju puni operativni dashboard. |
| Izrada prazne baze iz migracija i seed podataka | radi | Baza obrisana, API pokrenut na prazno — sve migracije (uključujući `AddDonations`, `AddAnimalMedia`, `AddUsersAndRoles`) primijenjene automatski, seed podaci i 4 demo korisnika kreirani. Ponovno pokretanje nije duplicirati ništa (provjereno brojem zapisa i ID-jem admina). |

## 2. Otkriveno i ispravljeno tijekom testiranja

- Izbornik za `Admin` i `Employee` prije je prikazivao i link **Volonteri** (popis volontera), što tražena tablica navigacije ne predviđa. Link je uklonjen iz te grupe kako bi izbornik tablično odgovarao specifikaciji zadatka. Stranica `/volunteers` i dalje postoji, samo više nije u izborniku.
- `IDialogService.ShowMessageBox` ne postoji u MudBlazor 9.5.0 — ispravan naziv je `ShowMessageBoxAsync`.

## 3. Tri sigurnosna ograničenja trenutne školske prijave

1. **Skriveni linkovi i query parametri nisu API autorizacija.** `NavMenu` sakriva stavke izbornika prema ulozi, a `MyTasks`/`MyDonations` filtriraju podatke preko `volunteerId`/`donorId` u URL-u. Ništa od toga ne sprječava korisnika da ručno pozove npr. `api/donations?donorId=5` ili `api/employees` izravno (preko Swaggera, curl-a ili mijenjanjem parametra u pregledniku) i dobije tuđe podatke — API trenutno ne provjerava tko je pozvao endpoint niti smije li vidjeti taj zapis.
2. **Prijava ne preživljava osvježavanje stranice ni novu karticu.** `CurrentUserService` je Scoped servis vezan uz trenutni Blazor Server circuit (SignalR vezu). Puna navigacija (npr. upisivanje URL-a, F5) otvara novi circuit i korisnik se tiho "odjavljuje" bez pravog logout eventa. To pokazuje da trenutno stanje prijave ne postoji ni u kolačiću ni u localStorageu — nema perzistentne sesije.
3. **Nema stvarne autentikacije na API sloju.** `AuthController.Login` samo provjerava lozinku i vraća `LoggedUserDto`; nakon toga API ne izdaje token niti prima ikakav dokaz identiteta u sljedećim zahtjevima. Svi API endpointi su trenutno potpuno otvoreni (nema `[Authorize]`), pa čak i neprijavljeni klijent može pozvati bilo koji CRUD endpoint.

**Prijedlog za pravu aplikaciju:** uvesti cookie ili JWT autentikaciju nakon uspješnog logina u `AuthController` (npr. `SignInAsync` s cookie shemom ili izdavanje JWT-a), dodati `[Authorize]` / `[Authorize(Roles = "...")]` atribute na kontrolere i akcije koje ne smiju biti javne (`DonorsController`, `EmployeesController`, `DonationsController` write akcije, `AnimalMediaController` upload/cover/delete), te u `MyTasks`/`MyDonations`/`AnimalProfile` provjeriti vlasništvo nad zapisom na API strani (npr. `donorId` iz tokena, a ne iz query parametra kojim klijent slobodno upravlja).
