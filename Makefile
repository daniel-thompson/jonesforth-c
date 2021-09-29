CC = gcc
CFLAGS = -Wall -Werror -g

ifdef NO_OPT
CFLAGS += -O0
else
CFLAGS += -O2
endif

OBJS = jonesforth.o

all : jonesforth

jonesforth : $(OBJS)
	$(CC) $(CFLAGS) -o $@ $(OBJS)

clean :
	$(RM) jonesforth debug.fs filetest.txt $(OBJS)

check : jonesforth
	cat jonesforth.fs selftest.fs | ./jonesforth
	@$(RM) filetest.txt

debug : jonesforth
	cat jonesforth.fs selftest.fs > debug.fs
	gdb jonesforth -ex "break go_forth" -ex "run < debug.fs"
