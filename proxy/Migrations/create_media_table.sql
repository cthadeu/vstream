create table media(
    id varchar(36) not null primary key,
    name varchar(255) not null,
    filename varchar(255) not null,
    status varchar(40),
    slug varchar(255),
    created_at timestamp not null
);
