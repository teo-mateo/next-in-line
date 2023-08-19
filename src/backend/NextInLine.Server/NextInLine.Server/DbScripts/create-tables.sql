-- Create the Item table
CREATE TABLE Items
(
    Id          SERIAL PRIMARY KEY,
    Name        VARCHAR(255) NOT NULL,
    WhenAdded   TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    AddedBy     VARCHAR(100) NOT NULL,
    Checked     BOOLEAN      NOT NULL DEFAULT FALSE,
    WhenChecked TIMESTAMP
);

-- Create the Tag table
CREATE TABLE Tags
(
    Id      SERIAL PRIMARY KEY,
    TagName VARCHAR(50) NOT NULL
);

CREATE TABLE ItemTags
(
    ItemId INTEGER NOT NULL,
    TagId  INTEGER NOT NULL,
    FOREIGN KEY (ItemId) REFERENCES Items (Id) ON DELETE CASCADE,
    FOREIGN KEY (TagId) REFERENCES Tags (Id) ON DELETE CASCADE,
    PRIMARY KEY (ItemId, TagId)
);