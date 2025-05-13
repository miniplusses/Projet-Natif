CREATE OR REPLACE PROCEDURE initialiser()
	LANGUAGE PLPGSQL
AS $$
BEGIN
	CREATE SCHEMA IF NOT EXISTS api;

	CREATE TABLE IF NOT EXISTS api.users (
		id UUID PRIMARY KEY,
		usernameApp TEXT NOT NULL,
		passwordApp TEXT NOT NULL,
		usernamePg TEXT NOT NULL,
		passwordPg TEXT NOT NULL,
		isAdmin BOOLEAN NOT NULL DEFAULT false,
		lastActivity TIMESTAMP NOT NULL DEFAULT NOW()
	);
	
	CREATE TABLE IF NOT EXISTS public.knowledges (
		id SERIAL PRIMARY KEY,
		title VARCHAR(255) NOT NULL,
		description TEXT NOT NULL,
		data JSONB NOT NULL
	);
	
	CREATE ROLE administrateur;
	
	GRANT USAGE, SELECT
	ON SEQUENCE knowledges_id_seq
	TO administrateur;
	
	GRANT USAGE 
	ON SCHEMA public
	TO administrateur;
	
	CREATE ROLE regulier;
	
	GRANT ALL
	ON public.knowledges
	TO administrateur;
	
	GRANT USAGE 
	ON SCHEMA public
	TO regulier;
	
	GRANT SELECT
	ON public.knowledges
	TO regulier;
END; $$;

CALL initialiser();

--- Créer un utilisateur postgresql avec ses informations de connexion
CREATE OR REPLACE FUNCTION api.insert_users(_usernameApp TEXT, _passwordApp TEXT, _passwordPg TEXT, _passwordPgClair TEXT)
	RETURNS BOOL
	LANGUAGE PLPGSQL
AS $$
DECLARE
    usernamePgGen VARCHAR(30);
BEGIN
	IF NOT EXISTS (SELECT 1 FROM api.users WHERE usernameApp = _usernameApp) THEN
        usernamePgGen := md5(random()::text || clock_timestamp()::text)::varchar(30);

        WHILE EXISTS (SELECT 1 FROM api.users WHERE usernamePg = usernamePgGen) LOOP
            usernamePgGen := md5(random()::text || clock_timestamp()::text)::varchar(30);
        END LOOP;
		
		INSERT INTO api.users (id, usernameApp, passwordApp, usernamePg, passwordPg) VALUES 
			(gen_random_uuid(), _usernameApp, _passwordApp, usernamePgGen, _passwordPg);

		EXECUTE 'CREATE ROLE "' || usernamePgGen || '" LOGIN PASSWORD ''' || _passwordPgClair || ''' IN ROLE regulier';

        RETURN TRUE;
    END IF;
    RETURN FALSE;
END; $$;

SELECT * FROM api.insert_users('admin', '$2a$11$qHjc5ae.Z5CSpSNKyuWSg.F7QMCgcm5EZR8Q356zN3Vwk7Nk9aCwu', 'DCC6914A245CDAF007957A3F470E455CE206F9B5815B2C153745E6A0A2F8511BDB3ED35A4B36CA4AD9848F9DAC0EA1622A6943F2B7FB31C5B449575719FE98E185E030E2DE64578568E611CDC1F7FC89493E1C851BD49F8AFE5C1DCD4C508B2C53A03EF3E55AAAD7E2AD000FA93CDF700975CEA65A1B826D9ADCF0D8275E5CACF8278D15439AB8FA6D6E7B7F75B8F91DE4DA80B11F9EC769D97BA80D4B8C9F5C72D5C677BF82F083D8E1BF0C664F46AADC94593A9C9B4D7B6E0EEEA6A85F64C3F7064283667E77C46D6C187C3CC90BB2974DAC800B5D425B80DB2CBF01062069C4A53570780331DDF59D24464E85CDFC2378CE4E44DCE65BB5756D920439FBA4D8F67D9E7601DA75F70AFFFF34A2DD39F64B4A37CC11C8586BF9C41A5C634DD7', 'MOddkv6CuStgbMaMiMH7Vawdxbyw86iTVjRNzPf3vOo4F3OKoNpV4KavqiCPnmph7ATHdfP4pERMtuQ8XKqxFSIKqnyEp2beUDjaFaI7bhlQUyXvCkzCZAggZk7ydDS1MeU0GJEF0iKJClzRLyJZxwfDpd1yXBIHmSXScI8j2BSMiKmZEoEaAVdUEDLEPNqRwfFyukL5S3dQVrH5ZkH3DkKxVIkK25EIuwlskR5g6DCfwoWrYCYi7Fl1KqMkhDaz');
SELECT * FROM api.insert_users('normal', '$2a$11$Icv1mjLdNgCij1KNX2XiaedQ9OqU31hbAO76g05LLfAZsEJ2htiLi', 'B9AA2EB45BDCBAFFA673C4AB06AE0E7BE9B3497FF4BDADF85E3A0307052C419913E8A9B7DAAA8846F9F4AE7C3DDF8AD7A46BC02DEAA0CE43226575356FD7CB9217A2B3E0AC1E888B44B72C0B1622A336A29785E469626D505D3084370D67DBA12340A507931243D1BEDAA6FCFFCA24DFA09D471C3E4CCE5FF6DC1411EEBB355F76B1FE9ED9692F189FF65F1077D9BFA40F613E967953DCD0BF4CAC7ACFACDAA3A2DDDF43916FBFACE688803D0A031C1A7BA85A9834253F93B2D6E71CF9C987F65EA27EB739990EC630CC981381ECBB24C6FD58F926112D11B346F99D404CD7582FA52B56CB7725D786BBB591B6A41A236560A551F2C1E194667676C7CFB97B7F09DFFCB096B84F3613ED3DF2DB8BD1A82B42A9698CD0F45517EAC5B79C2E5407', 'udiMVbDTCidMar1knc5JHQOwZadC0rZ4v0BEEMPdmcafyTGiDMU6wU5wxedheqHsgq6373VzrXZW4qq2Bh7ssnX7lu6420eqfgyvWUqQHgJl1ydfdbJWFXK6t8Scz2Ana2ZiE2YymhuuT3yL47Fmj210w1zjFGgStMNwdY3MiKKvIH2MRKfm6yA5ZpT8lqGYmF2I5Blex7mZ9G7BqNsgGfljWvfp6LzZRWdTWFanyNL0mzgyP1sybrkG843qSS0J');
UPDATE api.users SET isAdmin = true WHERE usernameapp = 'admin';


--- Suprimer le user postgresql
CREATE OR REPLACE FUNCTION delete_users()
	RETURNS TRIGGER 
	LANGUAGE PLPGSQL
AS $$
BEGIN
	IF OLD.lastActivity < NOW() - INTERVAL '7 days' THEN
		EXECUTE 'DROP ROLE IF EXISTS "' || OLD.usernamePg || '"';
        RETURN OLD;
   	END IF;
	RETURN NULL;
END;
$$;

CREATE OR REPLACE TRIGGER delete_users
    BEFORE DELETE ON api.users
    FOR EACH ROW
    EXECUTE FUNCTION delete_users();


-- Donne le role administrateur a l'utilisateur
CREATE OR REPLACE FUNCTION update_users()
	RETURNS TRIGGER 
	LANGUAGE PLPGSQL
AS $$
BEGIN
	IF OLD.isAdmin = false AND NEW.isAdmin = true THEN
		EXECUTE 'GRANT administrateur TO "' || OLD.usernamePg || '"';
		EXECUTE 'REVOKE regulier FROM "' || OLD.usernamePg || '"';
		RETURN NEW;
	END IF;
	
	IF Old.lastActivity != New.lastActivity THEN
		RETURN NEW;
	END IF;
	RETURN NULL;
END;
$$;

CREATE OR REPLACE TRIGGER update_users
    AFTER UPDATE ON api.users
    FOR EACH ROW
    EXECUTE FUNCTION update_users();
	
	
-- Recherche -- 
	
--------------- Ajouter la colonne de l'index
ALTER TABLE public.knowledges
    ADD COLUMN knowledges_index_col tsvector GENERATED ALWAYS AS (
        setweight(to_tsvector('french', coalesce(title,'')), 'A') ||
        setweight(to_tsvector('french', coalesce(description,'')), 'B') ||
        setweight(to_tsvector('french', coalesce(data::text,'')), 'C')) STORED;
		
--------------- Créer l'index
CREATE INDEX knowledges_idx ON public.knowledges USING GIN (knowledges_index_col);

	
-- Connaissance de jeux --

-- Jeu 1
INSERT INTO public.knowledges (title, description, data)
VALUES (
    'Valheim',
    'Tu es un viking, quoi de mieux?',
    '{
		"categorie": ["survival", "open world", "coop"],
		"prix": 22.79,
		"developpement":
			{
				"parution": "2021-02-02",
				"developpeur": "Iron Gate AB",
				"editeur": "Coffee Stain Publishing",
				"franchise": "Coffee Stain Publishing"
			},
		"evaluation": "Extrêmement positive",
		"platforme": "steam",
		"manette": "compatible",
		"complet": "false"}'
);

-- Jeu 2
INSERT INTO public.knowledges (title, description, data)
VALUES (
    'The Witcher 3: Wild Hunt',
    'Explorez un monde ouvert fantastique, rempli de monstres, de magie et d''intrigues captivantes.',
    '{
        "categorie": ["Action RPG", "Open World", "Fantasy"],
        "prix": 29.99,
        "developpement":
            {
                "parution": "2015-05-19",
                "developpeur": "CD Projekt",
                "editeur": "CD Projekt",
                "franchise": "The Witcher"
            },
        "evaluation": "Extrêmement positive",
        "platforme": "steam",
        "manette": "compatible",
        "complet": "true"}'
);

-- Jeu 3
INSERT INTO public.knowledges (title, description, data)
VALUES (
    'Counter-Strike: Global Offensive',
    'Rejoignez l''une des équipes et participez à des affrontements intenses dans ce jeu de tir à la première personne.',
    '{
        "categorie": ["FPS", "Multijoueur", "Tir tactique"],
        "prix": 14.99,
        "developpement":
            {
                "parution": "2012-08-21",
                "developpeur": "Valve",
                "editeur": "Valve",
                "franchise": "Counter-Strike"
            },
        "evaluation": "Très positive",
        "platforme": "steam",
        "manette": "compatible",
        "complet": "true"}'
);

-- Jeu 4
INSERT INTO public.knowledges (title, description, data)
VALUES (
    'Stardew Valley',
    'Gérez votre propre ferme, explorez des grottes, pêchez et socialisez avec les habitants dans ce jeu de simulation de vie.',
    '{
        "categorie": ["Simulation", "Indie", "RPG"],
        "prix": 14.99,
        "developpement":
            {
                "developpeur": "ConcernedApe"
            },
        "complet": "true"}'
);

-- Jeu 5
INSERT INTO public.knowledges (title, description, data)
VALUES (
    'DOOM Eternal',
    'Affrontez des hordes de démons dans ce jeu de tir à la première personne bourré d''action.',
    '{
        "categorie": ["FPS", "Action", "Gore"],
        "prix": 59.99,
        "developpement":
            {
                "parution": "2020-03-20",
                "developpeur": "id Software",
                "editeur": "Bethesda Softworks",
                "franchise": "DOOM"
            },
        "evaluation": "Extrêmement positive",
        "platforme": "steam",
        "manette": "compatible",
        "complet": "true"}'
);

