conn = new Mongo();
db = db.getSiblingDB('vstream');

db.createUser(
    {
        user: "vstream",
        pwd: "vstream",
        roles: [{ role: 'readWrite', db: 'vstream' }],
    }
);

db.createCollection("courses");
db.createCollection("modules");
db.createCollection("users");