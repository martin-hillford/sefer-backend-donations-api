-- This table contains all the donations made via the website
CREATE TABLE IF NOT EXISTS public.donations
(
    id            VARCHAR(36)   NOT NULL CONSTRAINT pk_donations PRIMARY KEY,
    creation_date TIMESTAMP     NOT NULL,
    currency      VARCHAR(5)    DEFAULT 'EUR' NOT NULL,
    provider      VARCHAR(255)  DEFAULT 'Mollie' NOT NULL,
    status        SMALLINT      NOT NULL,
    amount        INT           NOT NULL,
    payment_id    TEXT
);
