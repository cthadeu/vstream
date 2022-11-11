create table users (
	id varchar(36) not null primary key,
	name varchar(200) not null,
	email varchar(200) not null,
	password varchar(255) not null,
	phone varchar(30) null,
	user_type varchar(20) null,
	created_at timestamp default CURRENT_TIMESTAMP
);

create table user_courses (
	user_id varchar(36) not null,
	course_id varchar(36) not null,
	create_at timestamp default CURRENT_TIMESTAMP,
	primary key (user_id, course_id),
	foreign key (user_id) references users(id),
	foreign key (course_id) references courses(id)
);