-- This script contains the full schema of database as of migrations 38
-- but rewritten to Postgresql. Please note, that this script is not intended 
-- to run every deployment, though technically it is possible

-- This table contains all the donations made via the website
CREATE TABLE IF NOT EXISTS donations
(
    id            VARCHAR(36)   NOT NULL CONSTRAINT pk_donations PRIMARY KEY,
    creation_date TIMESTAMP     NOT NULL,
    status        SMALLINT      NOT NULL,
    amount        INT           NOT NULL,
    payment_id    TEXT
);