# AzilEdu

Sustav za upravljanje azilom za životinje — Blazor Server aplikacija (`AzilEdu.App`) s odvojenim ASP.NET Core Web API-jem (`AzilEdu.Api`), zajedničkim DTO/model projektom (`AzilEdu.Shared`) te JWT autentikacijom, autorizacijom po ulogama i AI funkcijama (Mock ili OpenAI provider).

## 1. Pokretanje API i App projekta

Potreban je .NET 10 SDK.

### API (`AzilEdu.Api`)

```powershell
cd AzilEdu.Api
dotnet run
```

- API sluša na `http://localhost:5086` (vidi `Properties/launchSettings.json`).
- Pri prvom pokretanju automatski se primjenjuju EF Core migracije nad lokalnom SQLite bazom (`AzilEdu.db`) i sjeju se demo podaci i demo korisnički računi.
- Swagger UI je dostupan na `http://localhost:5086/swagger` (samo u Development okruženju).

### App (`AzilEdu.App`)

```powershell
cd AzilEdu.App
dotnet run
```

- App sluša na `http://localhost:5062`.
- App očekuje da API već radi na `http://localhost:5086` (adresa je postavljena u `AzilEdu.App/Program.cs`).
- Otvori `http://localhost:5062/login` u pregledniku.

### Rebuild cijelog rješenja

```powershell
dotnet build AzilEdu.slnx
```

## 2. Demo računi

Svi demo računi imaju ulogu `User` plus jednu dodatnu poslovnu ulogu. Lozinke **nisu** navedene ovdje; nalaze se isključivo u `AzilEdu.Api/Data/AppUserSeeder.cs` i vrijede samo za lokalni razvoj — nisu prava produkcijska tajna i ne smiju se koristiti izvan lokalnog razvoja.

| Email                      | Uloge             | Povezani poslovni profil       |
| --------------------------- | ----------------- | ------------------------------- |
| `admin@aziledu.local`       | `User`, `Admin`    | —                                |
| `employee@aziledu.local`    | `User`, `Employee` | Djelatnik azila                 |
| `volunteer@aziledu.local`   | `User`, `Volunteer`| Volonter                        |
| `donor@aziledu.local`       | `User`, `Donor`    | Donator                         |

Administrator može kreirati dodatne račune (uključujući račune s više uloga istovremeno) na stranici `/users`.

## 3. Relacije `AppUser` ↔ poslovni entiteti

- **`AppUser` — `AppRole`**: veza je **mnogo-na-mnogo** preko povezne tablice `AppUserRole` (kompozitni ključ `AppUserId` + `AppRoleId`). Jedan račun tako može istovremeno imati više uloga (npr. `Employee` i `Donor`), što jedno polje `Role` na `AppUser` ne bi omogućilo.
- **`AppUser` — `Volunteer`**: **jedan-na-jedan (opcionalno)** preko nullable FK-a `AppUser.VolunteerId`. Račun ne mora biti povezan ni s jednim volonterskim profilom; brisanje volontera postavlja FK na `NULL` (`SetNull`), a ne briše sam korisnički račun.
- **`AppUser` — `Donor`**: isti obrazac kao gore, preko `AppUser.DonorId`.
- **`AppUser` — `Employee`**: isti obrazac kao gore, preko `AppUser.EmployeeId`.

`AppUser` time čuva samo identitet i ovlasti za prijavu; `Volunteer`, `Donor` i `Employee` ostaju odvojeni poslovni entiteti bez duplikata podataka.

## 4. Razlika između `401` i `403`

- **`401 Unauthorized`** — identitet zahtjeva nije (uspješno) potvrđen: token nedostaje, neispravan je, istekao je ili potpis ne odgovara. API ne zna tko šalje zahtjev.
- **`403 Forbidden`** — identitet je potvrđen (token je valjan), ali prijavljeni korisnik nema traženu ulogu, policy ili vlasništvo nad zapisom koji traži (npr. volonter pokušava dohvatiti tuđe zadatke).

## 5. AI endpointi i podaci koji se šalju provideru

Svi AI endpointi žive u `AzilEdu.Api/Controllers/AiController.cs` i zaštićeni su odgovarajućim `[Authorize]` pravilom. Blazor App nikad ne komunicira izravno s AI providerom — samo sa `AiController`om.

| Endpoint                                  | Ovlast                     | Podaci poslani AI servisu (`IAiService`)                                                                 |
| ------------------------------------------ | --------------------------- | ---------------------------------------------------------------------------------------------------------- |
| `GET /api/ai/status`                       | `Staff` (Admin/Employee)    | Ništa — vraća samo naziv providera/modela iz konfiguracije, nikad API ključ.                                |
| `POST /api/ai/text`                        | `Staff`                     | `Purpose` (jedna od `animal-adoption`, `donor-thank-you`, `social-post`) + slobodni `Input` tekst (do 4000 znakova) koji Blazor stranica sastavi od poslovnih polja (ime, vrsta, ton, iznos...). Nikad lozinke, `PasswordHash`, e-mail ni API ključevi.  |
| `GET /api/ai/daily-summary`                | `Staff`                     | Samo agregirani brojevi iz baze: ukupno životinja, dostupne za udomljenje, otvoreni/zakašnjeli zadaci, broj donacija u zadnjih 7 dana. Bez ijednog osobnog podatka.                                          |
| `GET /api/ai/volunteer-summary/mine`       | `Volunteer`                 | Do 10 vlastitih otvorenih zadataka prijavljenog volontera (naslov, tip, ime životinje, status, rok) — `VolunteerId` se čita isključivo iz JWT claima, nikad iz zahtjeva.                                     |
| `POST /api/ai/animal-intake`               | `Staff`                     | Slobodna terenska bilješka (do 4000 znakova) o pronađenoj životinji.                                        |
| `POST /api/ai/animal-data-check`           | `Staff`                     | Polja `SaveAnimalDto` (Name, Species, Breed, Gender, Age, ArrivalDate, AnimalStatusId, Description) — bez slika i bez internih ID-jeva baze.                                                                 |

## 6. Uključivanje Mock i OpenAI načina rada (bez commit-anja ključa)

Zadani način rada je `Mock` (`appsettings.json` → `Ai:Provider = "Mock"`) i radi potpuno bez interneta i bez API ključa.

Za prebacivanje na stvarni OpenAI provider, u `AzilEdu.Api` direktoriju **lokalno** postavi user secrets (ništa od ovoga se ne commita — `dotnet user-secrets` sprema podatke izvan repozitorija, u `%APPDATA%\Microsoft\UserSecrets`):

```powershell
cd AzilEdu.Api
dotnet user-secrets init
dotnet user-secrets set "Ai:Provider" "OpenAI"
dotnet user-secrets set "Ai:ApiKey" "OVDJE-IDE-LOKALNI-KLJUC"
dotnet user-secrets set "Ai:Model" "gpt-5.6-luna"
```

Ponovno pokreni API. Kartica na dashboardu (`Provider`/`Model`) i `GET /api/ai/status` sada moraju prikazati `OpenAI`. Za povratak na Mock:

```powershell
dotnet user-secrets set "Ai:Provider" "Mock"
```

Alternativa za CI/produkciju: postavi `Ai__Provider` i `Ai__ApiKey` kao environment varijable (dvostruka podvlaka umjesto dvotočke), nikad u `appsettings.json`.

## 7. Poznata ograničenja i prijedlozi za sljedeću verziju

**Poznata ograničenja:**

- `MockAiService`-ov "pametni unos" prepoznaje vrstu/spol samo jednostavnim podudaranjem ključnih riječi (npr. traži `"mačk"`) i osjetljiv je na dijakritike — bilješka bez hrvatskih znakova (npr. "macke" umjesto "mačke") neće biti prepoznata. Mock uopće ne pokušava izvući ime, pasminu ni starost; to je namjerno pojednostavljenje za rad bez ključa, a stvarni OpenAI provider to rješava bolje.
- `AnimalMediaController` provjerava samo deklarirani `Content-Type` iz multipart zahtjeva, ne i stvarni sadržaj datoteke (magic bytes) — teoretski je moguće poslati datoteku s krivim potpisom formata.
- Nema automatiziranih (xUnit/integracijskih) testova; sva autorizacijska i funkcionalna provjera u ovoj predaji rađena je ručno/skriptirano.
- Nema rate limitinga ni audit loga za AI pozive (spomenuto kao produkcijski zahtjev u checklisti, ali nije implementirano u ovoj verziji).
- Liste (`GET /api/animals`, `GET /api/donations`, ...) nemaju straničenje (pagination); pri velikom broju zapisa to postaje problem performansi.

**Prijedlozi za sljedeću verziju:**

1. Dodati xUnit projekt koji automatizira upravo ovakvu autorizacijsku matricu (401/403/200) po ulozi i endpointu te je uključiti u CI, umjesto ručnog PowerShell skriptiranja.
2. Dodati rate limiting po korisniku i audit log (bez zapisivanja tajnih podataka) za AI endpointe, te stvarnu validaciju sadržaja uploadane datoteke u `AnimalMediaController` (provjera magic bytes) prije spremanja na disk.
